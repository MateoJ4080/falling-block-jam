using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; set; }

    [SerializeField] public bool DebugMode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
