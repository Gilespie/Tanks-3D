using System;
using UnityEngine;

public enum WeatherType
{
    Rain,
    Snow,
    Clear,
    Clouds,
    CloudsMedium
}

public class Weather : MonoBehaviour
{
    [SerializeField] private float m_TimeToStage;
    [SerializeField] private WeatherType m_WeatherType;
    [SerializeField] private ParticleSystem m_Weather;
    [SerializeField, Range(0f, 1f)] private float m_PowerOfClouds;
    public WeatherType WeatherType => m_WeatherType;
    private bool _IsRain;
    private bool _IsSnow;

    void Start()
    {
        m_Weather = GetComponent<ParticleSystem>();
        WeatherController.WeatherChanged += OnWeatherChanged;
        m_WeatherType = WeatherType.Clear;
    }

    private void Update()
    {
        var emission = m_Weather.emission;
        emission.rateOverTimeMultiplier = Mathf.Clamp(emission.rateOverTimeMultiplier, 0f, 360f);
        emission.rateOverTimeMultiplier = m_PowerOfClouds * 360f;
    }

    private void OnWeatherChanged(WeatherType weatherType)
    {
        m_WeatherType = weatherType;
        _IsRain = m_WeatherType == WeatherType.Rain;
        _IsSnow = m_WeatherType == WeatherType.Snow;

        switch (weatherType)
        {
            case WeatherType.Rain:
                {
                    m_PowerOfClouds = 1f;
                    ActiveParticle();
                    break;
                }
            case WeatherType.Snow:
                {
                    m_PowerOfClouds = 1f;
                    ActiveParticle();
                    break;
                }
            case WeatherType.Clear:
                {
                    m_PowerOfClouds = 0f;
                    DeactiveParticle();
                    break;
                }
            case WeatherType.Clouds:
                {            
                    m_PowerOfClouds = UnityEngine.Random.Range(0.05f, 0.3f);
                    DeactiveParticle();
                    break;
                }
            case WeatherType.CloudsMedium:
                {        
                    m_PowerOfClouds = 0.5f;
                    DeactiveParticle();
                    break;
                }
        }
    }

    private void ActiveParticle()
    {
        if (_IsRain)
            m_Weather.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        if (_IsSnow)
            m_Weather.gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void DeactiveParticle()
    {
        if (!_IsRain)
            m_Weather.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        if (!_IsSnow)
            m_Weather.gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
}