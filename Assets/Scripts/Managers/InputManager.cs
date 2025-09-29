using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Piece.RotateRight.performed += ctx =>
        {
            if (GameManager.Instance.ActiveTetromino != null)
                GameManager.Instance.ActiveTetromino.GetComponent<Tetromino>().Rotate(-90);
        };

        controls.Piece.RotateLeft.performed += ctx =>
        {
            if (GameManager.Instance.ActiveTetromino != null)
                GameManager.Instance.ActiveTetromino.GetComponent<Tetromino>().Rotate(90);
        };
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        if (GameManager.Instance.ActiveTetromino == null) return;

        Vector2 input = controls.Piece.Move.ReadValue<Vector2>();
        GameManager.Instance.ActiveTetromino.GetComponent<Tetromino>().HandleMove();
    }
}
