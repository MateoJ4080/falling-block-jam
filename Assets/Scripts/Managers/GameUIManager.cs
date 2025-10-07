using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; set; }

    [Header("Headers")]
    [SerializeField] private TextMeshPro scoreTMP;
    [SerializeField] private TextMeshPro timeTMP;

    [Header("Containers")]
    [SerializeField] private GameObject nextContainer;
    [SerializeField] private GameObject holdContainer;

    [Header("References")]
    [SerializeField] private TetrominoSpawner spawner;

    private int score;
    private GameObject uiNext;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GameManager.Instance.OnLineCleared += AddScore;
        TimeManager.OnTimeChanged += UpdateTimeText;
        GameManager.Instance.OnNextTetrominoChanged += UpdateNextTetrominoUI;
    }

    void OnDisable()
    {
        GameManager.Instance.OnLineCleared -= AddScore;
        TimeManager.OnTimeChanged -= UpdateTimeText;
        GameManager.Instance.OnNextTetrominoChanged -= UpdateNextTetrominoUI;
    }

    private void AddScore(int value)
    {
        score += value;
        scoreTMP.text = score.ToString("D5");
    }

    private void UpdateTimeText(float value)
    {
        int minutes = Mathf.FloorToInt(value / 60);
        int seconds = Mathf.FloorToInt(value % 60);
        timeTMP.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateNextTetrominoUI()
    {
        if (uiNext != null) Destroy(uiNext);

        GameObject nextPrefab = GameManager.Instance.NextTetromino;

        uiNext = Instantiate(nextPrefab, nextContainer.transform);
        uiNext.transform.localPosition = Vector3.zero - GetUIPivotOffset(nextPrefab);
        uiNext.transform.rotation = Quaternion.Euler(-16.271f, 27.549f, -13.73f);
        uiNext.transform.localScale = Vector3.one * 0.36f;
        if (nextPrefab.name == "I_Tetromino") uiNext.transform.localScale = Vector3.one * 0.3f; // Less scale because this tetromino is wider

        Destroy(uiNext.GetComponent<Tetromino>());
    }

    public void UpdateHoldTetrominoUI(GameObject holdTetromino)
    {
        GameObject uiHold = holdTetromino;

        uiHold.transform.localPosition = Vector3.zero - GetUIPivotOffset(uiHold);
        uiHold.transform.rotation = Quaternion.Euler(-19.53f, -17.69f, 11.454f);
        uiHold.transform.localScale = Vector3.one * 0.36f;
        if (uiHold.name.StartsWith("I_Tetromino")) uiHold.transform.localScale = Vector3.one * 0.3f; // Less scale because this tetromino is wider

    }

    private Vector3 GetUIPivotOffset(GameObject tetromino)
    {
        Vector2 TileSize = GameManager.Instance.TileSize;
        float offsetX;
        float offsetY;

        string tetrominoName = tetromino.name.Replace("(Clone)", "").Trim();


        switch (tetrominoName)
        {
            case "I_Tetromino":
                offsetX = 0;
                offsetY = TileSize.y / 2;
                break;

            case "T_Tetromino":
            case "S_Tetromino":
            case "Z_Tetromino":
            case "J_Tetromino":
            case "L_Tetromino":
                offsetX = TileSize.x / 2;
                offsetY = 0;
                break;

            default:
                offsetX = 0f;
                offsetY = 0f;
                break;
        }

        return new(offsetX, offsetY);
    }
}

