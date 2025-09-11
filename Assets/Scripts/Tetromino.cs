using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Piece.RotateRight.performed += ctx => Rotate(-90);
        controls.Piece.RotateLeft.performed += ctx => Rotate(90);
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public void Rotate(int angle)
    {
        transform.Rotate(0, 0, angle);
    }
}
