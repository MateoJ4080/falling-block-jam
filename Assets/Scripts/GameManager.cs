using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Grid
    private Dictionary<Vector2Int, Transform> _gridState = new();
    [SerializeField] SpriteRenderer gridSr;
    public Vector2 GridBottomLeft { get; set; }

    [SerializeField] TetrominoSpawner spawner;
    [SerializeField] private float fallSpeed = 1f;
    public float FallSpeed => fallSpeed;

    public float TileSize { get; private set; }


    public GameObject ActiveTetromino { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        float tileWidth = gridSr.size.x / 10f;
        float tileHeight = gridSr.size.y / 20f;
        TileSize = Mathf.Min(tileWidth, tileHeight);

        GridBottomLeft = (Vector2)gridSr.transform.position - new Vector2(gridSr.size.x / 2f, gridSr.size.y / 2f);
        Debug.Log($"GridBottomLeft: {GridBottomLeft}");
    }

    public void SpawnNewTetromino()
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
            case "O_Tetromino":
                offsetY = TileSize;
                offsetX = 0;
                break;

            case "T_Tetromino":
            case "S_Tetromino":
            case "Z_Tetromino":
            case "J_Tetromino":
            case "L_Tetromino":
                offsetY = TileSize;
                offsetX = TileSize;
                break;

            default:
                Debug.LogWarning("Spawning with default offset");
                offsetY = TileSize;
                offsetX = 0f;
                break;
        }

        return gridSr.transform.position + (Vector3.up * (gridSr.size.y / 2f)) - (Vector3.right * offsetX) - (Vector3.up * offsetY);
    }


    public bool IsValidPosition(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= 10) return false;
        if (gridPos.y < 0 || gridPos.y >= 20) return false;

        if (_gridState.ContainsKey(gridPos)) return false;

        return true;
    }

    public void UpdateGridState(Vector2Int position, Transform block)
    {
        _gridState[position] = block;
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        float relativeX = (worldPos.x - GridBottomLeft.x) / TileSize;
        float relativeY = (worldPos.y - GridBottomLeft.y) / TileSize;

        int gridX = Mathf.FloorToInt(relativeX);
        int gridY = Mathf.FloorToInt(relativeY);

        return new Vector2Int(gridX, gridY);
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        float worldX = GridBottomLeft.x + gridPos.x * TileSize + TileSize / 2f;
        float worldY = GridBottomLeft.y + gridPos.y * TileSize + TileSize / 2f;

        return new Vector2(worldX, worldY);
    }
}
