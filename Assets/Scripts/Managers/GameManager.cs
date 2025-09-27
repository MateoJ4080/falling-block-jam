using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject ActiveTetromino { get; set; }
    public GameObject NextTetromino { get; set; }
    public GameObject HoldTetromino { get; set; }

    public float TileSize { get; private set; }
    public bool IsGameOver { get; set; }

    // Events
    public event Action<int> OnLineCleared;
    public event Action OnNextTetrominoChanged;

    // Grid
    [SerializeField] private SpriteRenderer gridSr;
    private readonly Dictionary<Vector2Int, Transform> _gridState = new();
    public Dictionary<Vector2Int, Transform> GridState => _gridState;
    public Vector2 GridBottomLeft { get; set; }

    // Tetrominoes
    [SerializeField] private GameObject[] tetrominoVisuals;
    [SerializeField] private TetrominoSpawner spawner;
    [SerializeField] private float fallSpeed = 1f;
    public float FallSpeed => fallSpeed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        float tileWidth = gridSr.size.x / 10f * gridSr.transform.localScale.x;
        float tileHeight = gridSr.size.y / 20f * gridSr.transform.localScale.y;
        TileSize = Mathf.Min(tileWidth, tileHeight);
        GridBottomLeft = (Vector2)gridSr.transform.position - new Vector2(gridSr.size.x * gridSr.transform.localScale.x / 2f, gridSr.size.y * gridSr.transform.localScale.y / 2f);

        AudioManager.Instance.PlayMusic(AudioManager.Instance.musicGameplay);

        SetNextTetromino();
    }

    void Start()
    {
        SpawnNewTetromino();
    }

    public void SpawnNewTetromino()
    {
        if (NextTetromino == null) Debug.Log("NextTetromino is null");
        spawner.SpawnTetromino(NextTetromino);
        SetNextTetromino();
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

        Vector3 gridScale = gridSr.transform.localScale;
        float gridHeight = gridSr.size.y * gridScale.y;

        return gridSr.transform.position + (Vector3.up * (gridHeight / 2f)) - (Vector3.right * offsetX) - (Vector3.up * offsetY);
    }


    public bool IsValidPosition(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x > 9) return false;
        if (gridPos.y < 0 || gridPos.y >= 19) return false;

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
        float worldX = GridBottomLeft.x + (gridPos.x * TileSize) + (TileSize / 2f);
        float worldY = GridBottomLeft.y + (gridPos.y * TileSize) + (TileSize / 2f);

        return new Vector2(worldX, worldY);
    }

    public bool IsLineComplete(int height)
    {
        for (int i = 1; i < 10; i++) // Might change "10" for "GridLength" in the future
        {
            if (!Instance.GridState.ContainsKey(new Vector2Int(i, height))) return false;
        }
        return true;
    }

    public bool IsLineEmpty(int height)
    {
        return !GridState.Keys.Any(pos => pos.y == height);
    }

    public void ClearLines(List<int> heights)
    {
        for (int x = 0; x < 10; x++)
        {
            foreach (int height in heights)
            {
                if (Instance.GridState.TryGetValue(new Vector2Int(x, height), out var block))
                {
                    Destroy(block.gameObject);
                    Instance.GridState.Remove(new Vector2Int(x, height));
                }
            }
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxClearLine);

        OnLineCleared.Invoke(10);

        DropLinesAbove(heights);
    }

    public void DropLinesAbove(List<int> heights)
    {
        int lowestClearedHeight = heights[0];

        // Move all lines above cleared rows down by appropriate amount
        for (int currentHeight = lowestClearedHeight + 1; currentHeight < 20; currentHeight++)
        {
            int tilesToDrop = heights.Count(i => i < currentHeight);

            if (!IsLineEmpty(currentHeight))
            {
                for (int x = 0; x < 10; x++)
                {
                    if (GridState.TryGetValue(new(x, currentHeight), out var block))
                    {
                        block.position = GridToWorld(new(x, currentHeight - tilesToDrop));
                        GridState.Add(new(x, currentHeight - tilesToDrop), block);

                        GridState.Remove(new(x, currentHeight));
                    }
                }
            }
        }
    }

    public IEnumerator CheckGameOver(Transform tetromino)
    {
        foreach (Transform block in tetromino)
        {
            Vector2Int gridSpawnPos = Instance.WorldToGrid(block.position);

            // Game over sequence
            if (Instance.GridState.ContainsKey(gridSpawnPos))
            {
                Instance.IsGameOver = true;

                AudioManager.Instance.musicSource.Stop();
                AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxGameOver);

                yield return new WaitForSeconds(AudioManager.Instance.sfxGameOver.length);
                SceneManager.LoadScene("MainMenu");
                yield break;
            }
        }
    }

    public void SetNextTetromino()
    {
        NextTetromino = spawner.tetrominos[UnityEngine.Random.Range(0, spawner.tetrominos.Length)];
        OnNextTetrominoChanged?.Invoke();
    }
}
