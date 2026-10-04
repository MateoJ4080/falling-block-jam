using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Piece.RotateRight.performed += ctx =>
        {
            if (GameManager.Instance.ActiveTetromino != null)
                GameManager.Instance.ActiveTetromino.GetComponent<Tetromino>().Rotate(-90);
        };

        _controls.Piece.RotateLeft.performed += ctx =>
        {
            if (GameManager.Instance.ActiveTetromino != null)
                GameManager.Instance.ActiveTetromino.GetComponent<Tetromino>().Rotate(90);
        };
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void Update()
    {
        if (GameManager.Instance.ActiveTetromino == null) return;

        Vector2 input = _controls.Piece.Move.ReadValue<Vector2>();
        GameManager.Instance.ActiveTetromino.GetComponent<Tetromino>().HandleMove();
    }
}
