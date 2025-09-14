using UnityEngine;
using UnityEngine.InputSystem;
public class Tetromino : MonoBehaviour
{
    private PlayerControls controls;
    private float lastUpdateTime;
    private Transform tetrominoTransform;
    private bool _isLocked;

    private void Awake()
    {
        tetrominoTransform = gameObject.transform;
        controls = new PlayerControls();

        controls.Piece.Move.performed += ctx => Move(ctx);
        controls.Piece.RotateRight.performed += ctx => Rotate(-90);
        controls.Piece.RotateLeft.performed += ctx => Rotate(90);
    }

    private void Start()
    {
        lastUpdateTime = Time.time;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (Time.time >= lastUpdateTime + GameManager.Instance.FallSpeed)
        {
            if (_isLocked) return;
            Fall();

            lastUpdateTime = Time.time;
        }
    }

    public void Fall()
    {
        if (Time.time >= lastUpdateTime + GameManager.Instance.FallSpeed && CanFall())
        {
            transform.position += Vector3.down * GameManager.Instance.TileSize;
        }
        else LockAndSpawnNew();
    }

    public void Move(InputAction.CallbackContext callbackContext)
    {
        if (_isLocked) return;

        Vector2 input = callbackContext.ReadValue<Vector2>();
        Vector2 move = Vector2.zero;

        // Avoid moving in two directions at the same time
        if (Mathf.Abs(input.x) > 0)
            move = new Vector2(Mathf.Sign(input.x), 0);
        else if (input.y < 0)
            move = new Vector2(0, -1);

        transform.position += (Vector3)(move * GameManager.Instance.TileSize);
    }

    public void Rotate(int angle)
    {
        if (_isLocked) return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxRotate, AudioManager.Instance.sfxVolume);
        transform.Rotate(0, 0, angle);
    }

    private bool CanFall()
    {
        foreach (Transform block in tetrominoTransform)
        {
            // Check if it's at the grid bottom or square below is occupied
            if (!GameManager.Instance.IsValidPositionToFall((Vector2)block.transform.position - Vector2.up * GameManager.Instance.TileSize)) return false;
        }

        return true;
    }

    private void LockAndSpawnNew()
    {
        _isLocked = true;

        foreach (Transform block in tetrominoTransform)
        {
            GameManager.Instance.UpdateGridState(block.transform.position, block);
        }

        GameManager.Instance.SpawnTetromino();
    }
}
