using System;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static event Action<WeatherType> WeatherChanged;
    public static float ThunderChance;

    [SerializeField] private float m_TimeToChangeWeather;
    [SerializeField] private bool m_IsTimerActive;

    private float timerChange;
    public float TimerChange => timerChange;

    private WeatherType randomIndex;

    private void Start()
    {
        timerChange = 0f;
        InvokeRepeating("ChangeThunderChance", 0f, 10f);
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
        randomIndex = (WeatherType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(WeatherType)).Length);
        WeatherChanged?.Invoke(randomIndex);
    }

    private float ChangeThunderChance()
    {
        return ThunderChance = UnityEngine.Random.Range(0f, 1f);
    }

/*
    private IEnumerator StartChangeChance()
    {
        while(true)
        {
            yield return ThunderChance;
            yield return new WaitForSeconds(1f);
        }
    }*/
}