using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static event Action<float> OnTimeChanged;
    private float time;

    private void Update()
    {
        time += Time.deltaTime;
        OnTimeChanged?.Invoke(time);
    }
}