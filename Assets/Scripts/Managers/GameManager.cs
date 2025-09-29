using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    private PlayerControls controls;

    // Events
    public event Action<int> OnLineCleared;
    public event Action OnNextTetrominoChanged;
    public event Action OnHoldTetrominoChanged;

    // Grid
    [SerializeField] private SpriteRenderer gridSr;
    private readonly Dictionary<Vector2Int, Transform> _gridState = new();
    public Dictionary<Vector2Int, Transform> GridState => _gridState;
    public Vector2 GridBottomLeft { get; set; }

    // Tetrominoes
    [SerializeField] private Transform holdContainer;
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
        controls = new PlayerControls();

        AudioManager.Instance.PlayMusic(AudioManager.Instance.musicGameplay);

        SetNextTetromino();
    }

    void Start()
    {
        SpawnNewTetromino();

        controls.Piece.SetHold.performed += ctx => SetHoldTetromino(ActiveTetromino);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
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
        for (int i = 0; i < 10; i++) // Might change "10" for "GridLength" in the future
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
        foreach (int height in heights)
        {
            for (int x = 0; x < 10; x++)
            {
                if (Instance.GridState.TryGetValue(new Vector2Int(x, height), out var block))
                {
                    Debug.Log($"Destroying block in ({x}, {height})");
                    Destroy(block.gameObject);
                    Instance.GridState.Remove(new Vector2Int(x, height));
                }
            }
            OnLineCleared.Invoke(10);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxClearLine);

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

    public void SetHoldTetromino(GameObject active)
    {
        if (HoldTetromino == null)
        {
            HoldTetromino = Instantiate(active, Vector3.zero, Quaternion.identity, holdContainer);
            HoldTetromino.transform.localPosition = Vector3.zero;
            Destroy(HoldTetromino.GetComponent<Tetromino>());
            Destroy(active);
            SpawnNewTetromino();
        }
        else
        {
            GameObject tempHold = HoldTetromino;
            GameObject tempActive = ActiveTetromino;

            Destroy(active);
            Destroy(HoldTetromino);

            // Switch Active to Hold
            Destroy(tempActive.GetComponent<Tetromino>());
            HoldTetromino = Instantiate(tempActive, Vector3.zero, Quaternion.identity, holdContainer);
            HoldTetromino.transform.localPosition = Vector3.zero;

            // Switch Hold to Actve
            ActiveTetromino = Instantiate(tempHold, GetSpawnPosition(tempHold), Quaternion.identity);
            ActiveTetromino.AddComponent<Tetromino>();
        }

        // OnHoldTetrominoChanged?.Invoke();
    }
}
