using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
public class Tetromino : MonoBehaviour
{
    private PlayerControls controls;
    private float lastUpdateTime;
    private Transform tetrominoTransform;


    private void Awake()
    {
        tetrominoTransform = gameObject.transform;
        controls = new PlayerControls();

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
            Fall();

            lastUpdateTime = Time.time;
        }
    }

    public void Fall()
    {
        if (Time.time >= lastUpdateTime + GameManager.Instance.FallSpeed && CanFall())
        {
            transform.position += Vector3.down * GameManager.Instance.TileSize;
            // GameManager.Instance.UpdateGridState();
        }
        else LockAndSpawnNew();
    }

    public void Rotate(int angle)
    {
        transform.Rotate(0, 0, angle);
    }

    private bool CanFall()
    {
        foreach (Transform block in tetrominoTransform)
        {
            Vector2Int blockPos = new(Mathf.RoundToInt(block.position.x), Mathf.RoundToInt(block.position.y));

            if (GameManager.Instance.IsValidPositionToFall(blockPos.x, blockPos.y - 1)) return false;
        }

        return true;
    }

    private void LockAndSpawnNew()
    {
        foreach (Transform block in tetrominoTransform)
        {
            Vector2 blockPos = block.transform.position;
            GameManager.Instance.AddBlock(block, (int)blockPos.x, (int)blockPos.y);
        }

        GameManager.Instance.ActiveTetromino = null;
        GameManager.Instance.SpawnTetromino();
    }
}
