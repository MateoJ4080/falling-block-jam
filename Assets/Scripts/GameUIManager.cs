using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; set; }

    [Header("Headers")]
    [SerializeField] TextMeshPro scoreTMP;
    [SerializeField] TextMeshPro timeTMP;

    private int score;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GameManager.Instance.OnLineCleared += UpdateScoreText;
        TimeManager.OnTimeChanged += UpdateTimeText;
    }

    void OnDisable()
    {
        GameManager.Instance.OnLineCleared -= UpdateScoreText;
        TimeManager.OnTimeChanged -= UpdateTimeText;
    }

    private void UpdateScoreText(int value)
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
}
