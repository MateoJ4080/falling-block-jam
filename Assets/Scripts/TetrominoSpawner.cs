using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] tetrominos;

    void Start()
    {
        SpawnRandom();
    }

    public void SpawnRandom()
    {
        if (GameManager.Instance.IsGameOver) return;

        // Random selection
        GameObject prefab = tetrominos[Random.Range(0, tetrominos.Length)];

        Vector3 spawnPos = GameManager.Instance.GetSpawnPosition(prefab);

        GameObject tetromino = Instantiate(prefab, spawnPos, Quaternion.identity);
        tetromino.transform.localScale = Vector3.one * GameManager.Instance.TileSize;

        GameManager.Instance.ActiveTetromino = tetromino;

        StartCoroutine(GameManager.Instance.CheckGameOver(tetromino.transform));
    }
}
