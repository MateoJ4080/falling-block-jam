using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] TetrominoSpawner spawner;
    [SerializeField] SpriteRenderer gridSr;

    [SerializeField] private float fallSpeed = 1f;
    public float FallSpeed => fallSpeed;

    private Transform[,] gridState = new Transform[10, 20];
    public float TileSize { get; private set; }


    public GameObject ActiveTetromino { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        float tileWidth = gridSr.size.x / 10f;
        float tileHeight = gridSr.size.y / 20f;
        TileSize = Mathf.Min(tileWidth, tileHeight);
    }

    public void SpawnTetromino()
    {
        spawner.SpawnRandom();
    }

    public Vector3 GetSpawnPosition(GameObject prefab)
    {
        float offsetX;
        float offsetY;

        switch (prefab.name)
        {
            case "I_Tetromino":
                offsetY = TileSize / 2f;
                offsetX = 0f;
                break;

            case "T_Tetromino":
            case "S_Tetromino":
            case "Z_Tetromino":
            case "J_Tetromino":
            case "L_Tetromino":
                offsetY = TileSize;
                offsetX = TileSize * 0.5f;
                break;

            case "O_Tetromino":
                offsetY = TileSize;
                offsetX = 0;
                break;

            default:
                Debug.LogWarning("Spawning with default pffset");
                offsetY = TileSize;
                offsetX = 0f;
                break;
        }

        return gridSr.transform.position + (Vector3.up * (gridSr.size.y / 2f)) - (Vector3.right * offsetX) - (Vector3.up * offsetY);
    }

    public void AddBlock(Transform block, int x, int y) => gridState[x, y] = block;

    public bool IsValidPositionToFall(int x, int y)
    {
        if (x < 0 || x >= 10 || y < 0 || y >= 20)
        {
            Debug.LogWarning($"IsOccupied: index out of bounds for x={x}, y={y}");
            return false;
        }
        Debug.Log("Position below occupied. Locking.");
        return gridState[x, y] != null;
    }
}
