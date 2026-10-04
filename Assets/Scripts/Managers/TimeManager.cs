using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static event Action<float> OnTimeChanged;
    private float _time;

    private void Update()
    {
        _time += Time.deltaTime;
        OnTimeChanged?.Invoke(_time);
    }
}