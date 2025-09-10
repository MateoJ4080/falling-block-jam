using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameManager Instance { get; set; }

    public GameObject ActivePiece { get; private set; }
    public float FallSpeed { get; private set; } = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnNewPiece(GameObject piecePrefab, Vector2 position)
    {
        GameObject piece = Instantiate(piecePrefab, position, Quaternion.identity);
        ActivePiece = piece;
    }

    public void UpdateScore(int points)
    {
        // BoardMana
    }
}
