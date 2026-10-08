using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject ActiveTetromino { get; set; }
    public GameObject NextTetromino { get; set; }
    public GameObject HoldTetromino { get; set; }

    public Vector2 TileSize { get; private set; }
    public bool IsGameOver { get; set; }
    private PlayerControls _controls;
    private bool _isHoldAvailable;

    // Events
    public event Action<int> OnLineCleared;
    public event Action OnNextTetrominoChanged;

    // Grid
    [SerializeField] private SpriteRenderer _gridSr;
    private readonly Dictionary<Vector2Int, Transform> _gridState = new();
    public Dictionary<Vector2Int, Transform> GridState => _gridState;
    public Vector2 GridBottomLeft { get; set; }

    // Tetrominoes
    [SerializeField] private Transform _holdContainer;
    [SerializeField] private TetrominoSpawner _spawner;
    [SerializeField] private float _fallSpeed = 1f;
    public float FallSpeed => _fallSpeed;

    // Buttons for mobile
    [SerializeField] private MoveButton _moveButtonLeft;
    [SerializeField] private MoveButton _moveButtonRight;
    [SerializeField] private MoveButton _moveButtonDown1;
    [SerializeField] private MoveButton _moveButtonDown2;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        float tileWidth = _gridSr.bounds.size.x / 10f;
        float tileHeight = _gridSr.bounds.size.y / 20f;
        TileSize = new(tileWidth, tileHeight);
        GridBottomLeft = (Vector2)_gridSr.transform.position - new Vector2(_gridSr.size.x * _gridSr.transform.localScale.x / 2f, _gridSr.size.y * _gridSr.transform.localScale.y / 2f);
        _controls = new PlayerControls();

        AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicGameplay);

        SetNextTetromino();
    }

    private void Start()
    {
        SpawnNewTetromino();

        _controls.Piece.SetHold.performed += ctx => SetHoldTetromino();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    public void SpawnNewTetromino()
    {
        if (NextTetromino == null) Debug.Log("NextTetromino is null");

        _isHoldAvailable = true;
        _spawner.SpawnTetromino(NextTetromino);
        SetNextTetromino();

        if (_moveButtonLeft != null) _moveButtonLeft.tetromino = ActiveTetromino.GetComponent<Tetromino>();
        if (_moveButtonRight != null) _moveButtonRight.tetromino = ActiveTetromino.GetComponent<Tetromino>();
        if (_moveButtonDown1 != null) _moveButtonDown1.tetromino = ActiveTetromino.GetComponent<Tetromino>();
        if (_moveButtonDown2 != null) _moveButtonDown2.tetromino = ActiveTetromino.GetComponent<Tetromino>();
    }

    public void RotateCurrent(int angle)
    {
        if (ActiveTetromino != null)
            ActiveTetromino.GetComponent<Tetromino>().Rotate(angle);
    }

    public Vector2 GetSpawnPosition()
    {
        Bounds bounds = _gridSr.bounds;
        return new Vector2(bounds.center.x, bounds.max.y) - Vector2.up * TileSize.y;
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
        float relativeX = (worldPos.x - GridBottomLeft.x) / TileSize.x;
        float relativeY = (worldPos.y - GridBottomLeft.y) / TileSize.y;

        int gridX = Mathf.FloorToInt(relativeX);
        int gridY = Mathf.FloorToInt(relativeY);

        return new Vector2Int(gridX, gridY);
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        float worldX = GridBottomLeft.x + (gridPos.x * TileSize.x) + (TileSize.x / 2f);
        float worldY = GridBottomLeft.y + (gridPos.y * TileSize.y) + (TileSize.y / 2f);

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
                    Destroy(block.gameObject);
                    Instance.GridState.Remove(new Vector2Int(x, height));
                }
            }
            OnLineCleared.Invoke(10);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.SfxClearLine);

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

    public bool CheckGameOver(Transform tetromino, Vector2 spawnPos)
    {
        foreach (Transform block in tetromino)
        {
            Vector2 worldPos = spawnPos + Vector2.Scale(block.localPosition, TileSize);
            Vector2Int gridPos = WorldToGrid(worldPos);

            if (Instance.GridState.ContainsKey(gridPos))
                return true;
        }

        return false;
    }

    public IEnumerator GameOverSequence()
    {
        IsGameOver = true;

        AudioManager.Instance.MusicSource.Stop();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.SfxGameOver);

        yield return new WaitForSeconds(AudioManager.Instance.SfxGameOver.length);

        SceneManager.LoadScene("Menu");
    }

    public void SetNextTetromino()
    {
        NextTetromino = _spawner.tetrominos[UnityEngine.Random.Range(0, _spawner.tetrominos.Length)];
        OnNextTetrominoChanged?.Invoke();
    }

    public void SetHoldTetromino()
    {
        GameObject active = ActiveTetromino;
        if (!_isHoldAvailable) return;

        if (HoldTetromino == null)
        {
            HoldTetromino = Instantiate(active, Vector3.zero, Quaternion.identity, _holdContainer);
            HoldTetromino.transform.localPosition = Vector3.zero;

            FixBlocksRotation(HoldTetromino);
            GameUIManager.Instance.UpdateHoldTetrominoUI(HoldTetromino);
            Destroy(HoldTetromino.GetComponent<Tetromino>());

            Destroy(active);
            SpawnNewTetromino();

            _isHoldAvailable = false;
        }
        else
        {
            GameObject tempHold = HoldTetromino;
            GameObject tempActive = ActiveTetromino;

            Destroy(active);
            Destroy(HoldTetromino);

            // Switch Active to Hold
            Destroy(tempActive.GetComponent<Tetromino>());
            HoldTetromino = Instantiate(tempActive, Vector3.zero, Quaternion.identity, _holdContainer);
            HoldTetromino.transform.localPosition = Vector3.zero;

            FixBlocksRotation(HoldTetromino);
            GameUIManager.Instance.UpdateHoldTetrominoUI(HoldTetromino);

            // Switch Hold to Actve
            ActiveTetromino = Instantiate(tempHold, GetSpawnPosition(), Quaternion.identity);
            ActiveTetromino.transform.localScale = Vector3.one * TileSize;
            ActiveTetromino.AddComponent<Tetromino>();

            if (_moveButtonLeft != null) _moveButtonLeft.tetromino = ActiveTetromino.GetComponent<Tetromino>();
            if (_moveButtonRight != null) _moveButtonRight.tetromino = ActiveTetromino.GetComponent<Tetromino>();
            if (_moveButtonDown1 != null) _moveButtonDown1.tetromino = ActiveTetromino.GetComponent<Tetromino>();
            if (_moveButtonDown2 != null) _moveButtonDown2.tetromino = ActiveTetromino.GetComponent<Tetromino>();

            _isHoldAvailable = false;
        }

        // OnHoldTetrominoChanged?.Invoke();
    }

    private void FixBlocksRotation(GameObject tetromino)
    {
        foreach (Transform block in tetromino.transform)
        {
            block.rotation = Quaternion.identity;
        }
    }
}
