using UnityEngine;
using UnityEngine.InputSystem;
public class Tetromino : MonoBehaviour
{
    private PlayerControls controls;
    private float lastUpdateTime;
    private bool _isLocked;

    private void Awake()
    {
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
        Fall();
    }

    public void Fall()
    {
        if (_isLocked) return;

        if (Time.time >= lastUpdateTime + GameManager.Instance.FallSpeed)
        {
            if (CanFall())
            {
                transform.position += Vector3.down * GameManager.Instance.TileSize;
                lastUpdateTime = Time.time;
            }
            else
            {
                LockAndSpawnNew();
            }
        }
    }

    public void Move(InputAction.CallbackContext callbackContext)
    {
        if (_isLocked) return;

        Vector2 input = callbackContext.ReadValue<Vector2>();
        Vector2 direction = Vector2.zero;

        // Avoid moving in two directions at the same time
        if (Mathf.Abs(input.x) > 0)
            direction = new Vector2(Mathf.Sign(input.x), 0);
        else if (input.y < 0)
            direction = new Vector2(0, -1);

        if (CanMoveTo(direction)) transform.position += (Vector3)(direction * GameManager.Instance.TileSize);
    }

    public void Rotate(int angle)
    {
        if (_isLocked) return;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxRotate, AudioManager.Instance.sfxVolume);
        transform.Rotate(0, 0, angle);
    }

    private bool CanFall()
    {
        foreach (Transform block in transform)
        {
            // Check if it's at the grid bottom or square below is occupied
            Vector2 worldPos = (Vector2)block.position - new Vector2(0, GameManager.Instance.TileSize);
            Vector2Int gridPos = GameManager.Instance.WorldToGrid(worldPos);

            if (!GameManager.Instance.IsValidPosition(gridPos)) return false;
        }
        return true;
    }

    private bool CanMoveTo(Vector3 direction)
    {
        foreach (Transform block in transform)
        {
            // Check if square of the grid at "direction" is occupied
            Vector2 worldPos = (Vector2)block.position + (Vector2)(direction * GameManager.Instance.TileSize);
            Vector2Int gridPos = GameManager.Instance.WorldToGrid(worldPos);

            if (!GameManager.Instance.IsValidPosition(gridPos)) return false;
        }
        return true;
    }


    private void LockAndSpawnNew()
    {
        _isLocked = true;

        foreach (Transform block in transform)
        {
            Vector2Int gridPos = GameManager.Instance.WorldToGrid(block.transform.position);
            Debug.Log($"Dictionary updated: {gridPos}, {block}");
            GameManager.Instance.UpdateGridState(gridPos, block);
        }

        GameManager.Instance.SpawnNewTetromino();
    }
}
