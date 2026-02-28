using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Screen.width / Screen.height;

        Vector2 spriteSize = sr.sprite.bounds.size;

        float scaleX = worldWidth / spriteSize.x;
        float scaleY = worldHeight / spriteSize.y;

        float scale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}