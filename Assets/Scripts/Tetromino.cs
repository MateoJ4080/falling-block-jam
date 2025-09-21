using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
public class Tetromino : MonoBehaviour
{
    private PlayerControls controls;
    private float lastUpdateTime;
    private float lastMoveTime;
    private bool _isLocked;

    enum BoundCheckResult
    {
        Inside,
        OutLeft,
        OutRight,
        OutBottom,
        OutTop,
        Overlapping
    }

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Piece.RotateRight.performed += ctx => Rotate(-90);
        controls.Piece.RotateLeft.performed += ctx => Rotate(90);
    }
    private void Start()
    {
        lastUpdateTime = Time.time;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        HandleMove();
        Fall();
    }

    public void Fall()
    {
        if (_isLocked) return;

        if (Time.time >= lastUpdateTime + GameManager.Instance.FallSpeed)
        {
            if (CanFall())
            {
                Debug.LogWarning($"Falling to {transform.position + Vector3.down * GameManager.Instance.TileSize}");

                transform.position += Vector3.down * GameManager.Instance.TileSize;
                lastUpdateTime = Time.time;
            }
            else
            {
                LockTetromino();
            }
        }
    }

    public void HandleMove()
    {
        if (_isLocked) return;

        Vector2 input = controls.Piece.Move.ReadValue<Vector2>();
        Vector2 direction = Vector2.zero;

        if (Mathf.Abs(input.x) > 0) direction.x = Mathf.Sign(input.x);
        if (input.y < 0) direction.y = -1;

        if (direction != Vector2.zero && Time.time >= lastMoveTime + 0.1f)
        {
            if (CanMoveTo(direction))
            {
                transform.position += (Vector3)(direction * GameManager.Instance.TileSize);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxMove);
            }
            lastMoveTime = Time.time;
        }
    }

    public void Rotate(int angle)
    {
        if (_isLocked) return;
        bool canRotate = true;

        // Check if out of bounds and reposition
        foreach (Transform block in transform)
        {
            var result = CheckBlockAfterRotation(block, transform, angle);

            if (result == BoundCheckResult.Overlapping)
            {
                canRotate = false;
                break;
            }
            else if (result != BoundCheckResult.Inside)
            {
                switch (result)
                {
                    case BoundCheckResult.OutLeft:
                        transform.position += Vector3.right * GameManager.Instance.TileSize;
                        break;
                    case BoundCheckResult.OutRight:
                        transform.position += Vector3.left * GameManager.Instance.TileSize;
                        break;
                    case BoundCheckResult.OutBottom:
                        transform.position += Vector3.up * GameManager.Instance.TileSize;
                        break;
                    case BoundCheckResult.OutTop:
                        transform.position += Vector3.down * GameManager.Instance.TileSize;
                        break;
                }
                break;
            }
        }

        // Apply rotation and sfx
        if (canRotate)
        {
            transform.Rotate(0, 0, angle);

            foreach (Transform block in transform)
            {
                block.Rotate(0, 0, -angle);
            }

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxRotate);
        }
    }

    BoundCheckResult CheckBlockAfterRotation(Transform block, Transform parent, float angle)
    {
        Vector3 dir = block.position - parent.position;
        dir = Quaternion.Euler(0, 0, angle) * dir;
        Vector2Int gridPos = GameManager.Instance.WorldToGrid(parent.position + dir);

        int x = gridPos.x;
        int y = gridPos.y;

        // Check if out of bounds
        if (x < 0) return BoundCheckResult.OutLeft;
        if (x >= 10) return BoundCheckResult.OutRight;
        if (y < 0) return BoundCheckResult.OutBottom;
        if (y >= 20) return BoundCheckResult.OutTop;

        // Check if overlapping another block
        if (GameManager.Instance.GridState.ContainsKey(new(x, y))) return BoundCheckResult.Overlapping;

        return BoundCheckResult.Inside;
    }

    private bool CanFall()
    {
        foreach (Transform block in transform)
        {
            // Check if it's at the grid bottom or square below is occupied
            Vector2 worldPos = (Vector2)block.position - new Vector2(0, GameManager.Instance.TileSize);
            Vector2Int gridPos = GameManager.Instance.WorldToGrid(worldPos);

            Debug.Log($"IsValid: Checking {gridPos} for {block.name}");

            if (!GameManager.Instance.IsValidPosition(gridPos))
            {
                return false;
            }
        }
        return true;
    }

    private bool CanMoveTo(Vector3 direction)
    {
        foreach (Transform block in transform)
        {
            // Check if square of the grid at "direction" is occupied
            Vector2 worldPos = (Vector2)block.position + (Vector2)(direction * GameManager.Instance.TileSize);
            Vector2Int gridPos = GameManager.Instance.WorldToGrid(worldPos);

            if (!GameManager.Instance.IsValidPosition(gridPos)) return false;
        }
        return true;
    }


    private void LockTetromino()
    {
        _isLocked = true;

        List<int> completedHeights = new();

        foreach (Transform block in transform)
        {
            Vector2Int gridPos = GameManager.Instance.WorldToGrid(block.transform.position);

            // Update GridState dictionary
            GameManager.Instance.UpdateGridState(gridPos, block);

            // Check for completed line
            if (GameManager.Instance.IsLineComplete(gridPos.y))
            {
                if (!completedHeights.Contains(gridPos.y)) completedHeights.Add(gridPos.y);
                completedHeights.Sort();
            }
        }

        if (completedHeights.Count != 0) GameManager.Instance.ClearLines(completedHeights);
        GameManager.Instance.SpawnNewTetromino();
    }
}
