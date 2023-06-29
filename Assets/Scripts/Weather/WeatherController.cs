using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeatherController : MonoBehaviour
{
    public static event Action<WeatherType> WeatherChanged;

    [SerializeField] private float m_TimeToChangeWeather;
    [SerializeField] private bool m_IsTimerActive;

    private float timerChange;
    public float TimerChange => timerChange;

    private WeatherType randomIndex;

    private void Start()
    {
        timerChange = 0f;
    }

    private void Update()
    {
        if (m_IsTimerActive)
        {
            timerChange += Time.deltaTime;

            if (timerChange >= m_TimeToChangeWeather)
            {
                ChangeWeather();
                timerChange = 0f;
            }
        }
    }

    private void ChangeWeather()
    {
        randomIndex = (WeatherType)Random.Range(0, Enum.GetValues(typeof(WeatherType)).Length);
        WeatherChanged?.Invoke(randomIndex);
        Debug.Log(randomIndex);
    }
}