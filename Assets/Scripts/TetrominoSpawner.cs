using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    public GameObject[] tetrominos;

    public void SpawnTetromino(GameObject prefab)
    {
        if (GameManager.Instance.IsGameOver) return;

        Vector2 spawnPos = GameManager.Instance.GetSpawnPosition();

        GameObject tetromino = Instantiate(prefab, spawnPos, Quaternion.identity);
        tetromino.transform.localScale = Vector3.one * GameManager.Instance.TileSize;

        GameManager.Instance.ActiveTetromino = tetromino;

        if (GameManager.Instance.CheckGameOver(prefab.transform, spawnPos))
        {
            StartCoroutine(GameManager.Instance.GameOverSequence());
            return;
        }
    }
}
