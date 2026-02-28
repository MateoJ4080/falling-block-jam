using UnityEngine;
using UnityEngine.EventSystems;

public class MoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Tetromino tetromino;
    public Vector2 direction;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tetromino != null)
            tetromino.StartMove(direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (tetromino != null)
            tetromino.StopMove();
    }
}