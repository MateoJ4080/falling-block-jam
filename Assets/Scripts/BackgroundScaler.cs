using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Screen.width / Screen.height;

        float scaleX = worldWidth / sr.bounds.size.x;
        float scaleY = worldHeight / sr.bounds.size.y;

        float scale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
