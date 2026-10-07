using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; set; }

    private int _score;
    private GameObject _uiNext;

    [Header("Headers")]
    [SerializeField] private TextMeshPro _scoreTMP;
    [SerializeField] private TextMeshPro _timeTMP;

    [Header("Containers")]
    [SerializeField] private GameObject _nextContainer;

    [Header("References")]
    [SerializeField] private TetrominoSpawner _spawner;
    [SerializeField] private GameObject _mobileControlsPanel;
    public bool IsMobileControlsPanelActive
    {
        get => _mobileControlsPanel.activeSelf;
        set => _mobileControlsPanel.SetActive(value);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (Application.isMobilePlatform)
            _mobileControlsPanel.SetActive(true);
        else
            _mobileControlsPanel.SetActive(false);

        GameManager.Instance.OnLineCleared += AddScore;
        TimeManager.OnTimeChanged += UpdateTimeText;
        GameManager.Instance.OnNextTetrominoChanged += UpdateNextTetrominoUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnLineCleared -= AddScore;
        TimeManager.OnTimeChanged -= UpdateTimeText;
        GameManager.Instance.OnNextTetrominoChanged -= UpdateNextTetrominoUI;
    }

    private void AddScore(int value)
    {
        _score += value;
        _scoreTMP.text = _score.ToString("D5");
    }

    private void UpdateTimeText(float value)
    {
        int minutes = Mathf.FloorToInt(value / 60);
        int seconds = Mathf.FloorToInt(value % 60);
        _timeTMP.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateNextTetrominoUI()
    {
        if (_uiNext != null) Destroy(_uiNext);

        GameObject nextPrefab = GameManager.Instance.NextTetromino;

        _uiNext = Instantiate(nextPrefab, _nextContainer.transform);
        _uiNext.transform.localPosition = Vector3.zero - GetUIPivotOffset(nextPrefab);
        _uiNext.transform.rotation = Quaternion.Euler(-19.53f, 27.549f, -13.73f);
        _uiNext.transform.localScale = Vector3.one * 0.48f;
        if (nextPrefab.name == "I_Tetromino") _uiNext.transform.localScale = Vector3.one * 0.377f; // Less scale because this tetromino is wider

        Destroy(_uiNext.GetComponent<Tetromino>());
    }

    public void UpdateHoldTetrominoUI(GameObject holdTetromino)
    {
        GameObject uiHold = holdTetromino;

        uiHold.transform.localPosition = Vector3.zero - GetUIPivotOffset(uiHold);
        uiHold.transform.rotation = Quaternion.Euler(-19.53f, -27.549f, 13.73f);
        uiHold.transform.localScale = Vector3.one * 0.48f;
        if (uiHold.name.StartsWith("I_Tetromino")) uiHold.transform.localScale = Vector3.one * 0.377f; // Less scale because this tetromino is wider

    }

    private Vector3 GetUIPivotOffset(GameObject tetromino)
    {
        float offsetX;
        float offsetY;

        string tetrominoName = tetromino.name.Replace("(Clone)", "").Trim();

        switch (tetrominoName)
        {
            case "I_Tetromino":
                offsetX = 0;
                offsetY = 0.2f;
                break;

            case "T_Tetromino":
            case "S_Tetromino":
            case "Z_Tetromino":
            case "J_Tetromino":
            case "L_Tetromino":
                offsetX = 0.2f;
                offsetY = 0;
                break;

            default:
                offsetX = 0f;
                offsetY = 0f;
                break;
        }

        return new(offsetX, offsetY);
    }

    public void OnMenuButtonPressed()
    {
        AudioManager.Instance.PlayButtonSFX();
        SceneManager.LoadScene(0);
    }
}

