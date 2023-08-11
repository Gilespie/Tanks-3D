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
    [SerializeField] private float m_ParticleAmount;
    [SerializeField] private SFXController m_SFXController;

    public WeatherType WeatherType => m_WeatherType;
    private bool _IsRain;
    private bool _IsSnow;
    private bool _IsNight;
    private bool _IsThunder;
    private float currentTime;
    private float startNight = 1150f;
    private float endNight = 300f;


    void Start()
    {
        m_Weather = GetComponent<ParticleSystem>();
        WeatherController.WeatherChanged += OnWeatherChanged;
        DayTime.DayTimeChanged += OnEnvironmentChanged;
        m_WeatherType = WeatherType.Clear;
    }

    private void Update()
    {
        var emission = m_Weather.emission;
        emission.rateOverTimeMultiplier = Mathf.Clamp(emission.rateOverTimeMultiplier, 0f, m_ParticleAmount);
        emission.rateOverTimeMultiplier = m_PowerOfClouds * m_ParticleAmount;
    }

    private void OnDestroy()
    {
        WeatherController.WeatherChanged -= OnWeatherChanged;
        DayTime.DayTimeChanged -= OnEnvironmentChanged;
    }

    private void OnWeatherChanged(WeatherType weatherType)
    {
        m_WeatherType = weatherType;
        _IsRain = m_WeatherType == WeatherType.Rain;
        _IsSnow = m_WeatherType == WeatherType.Snow;
        _IsThunder = WeatherController.ThunderChance > 0.5f && m_WeatherType == WeatherType.Rain || m_WeatherType == WeatherType.CloudsMedium;

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

    private void OnEnvironmentChanged(float time)
    {
        currentTime = time;
        _IsNight = currentTime >= startNight || currentTime <= endNight;

        if (_IsNight == true)
        {
            ActiveParticle();
        }
        else
        {
            DeactiveParticle();
        }
    }

    private void ActiveParticle()
    {
        if (_IsRain)
        {
            m_Weather.gameObject.transform.GetChild(0).gameObject.SetActive(true);
           
        }

        if (_IsSnow)
        {
            m_Weather.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            
        }

        if (_IsNight)
        {
            m_Weather.gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }

        if (_IsThunder)
        {
            m_Weather.gameObject.transform.GetChild(3).gameObject.SetActive(true);
            
        }
    }

    private void DeactiveParticle()
    {
        if (!_IsRain)
        {
            m_Weather.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            
        }

        if (!_IsSnow)
        {
            m_Weather.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            
        }

        if (!_IsNight)
        {
            m_Weather.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        }

        if (!_IsThunder)
        {
            m_Weather.gameObject.transform.GetChild(3).gameObject.SetActive(false);
            
        }
    }
}