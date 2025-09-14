using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Grid
    private Dictionary<Vector2, Transform> gridState = new();
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
                Debug.LogWarning("Spawning with default pffset");
                offsetY = TileSize;
                offsetX = 0f;
                break;
        }

        return gridSr.transform.position + (Vector3.up * (gridSr.size.y / 2f)) - (Vector3.right * offsetX) - (Vector3.up * offsetY);
    }

    public void UpdateGridState(Vector2 position, Transform block)
    {
        gridState[position] = block;
    }


    public bool IsValidPositionToFall(Vector2 fallPos)
    {
        if (fallPos.y <= GridBottomLeft.y + TileSize / 2)
        {
            Debug.LogWarning($"IsValidPositionToFall: index out of bounds for y: {fallPos.y}. Returning false");
            return false;
        }

        return true;
    }
}
