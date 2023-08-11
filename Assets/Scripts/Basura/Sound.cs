using UnityEngine;

public enum SoundsType
{
    Rain,
    Snow,
    Wind,
    Thunder
}

public class Sound : MonoBehaviour
{
    [SerializeField] private SoundsType m_Type;
    [SerializeField] private AudioSource m_WeatherSFX;

    public void PlaySXF()
    {
        switch(m_Type)
        {
            case SoundsType.Rain:
                {
                    m_WeatherSFX.Play();
                    break;
                }
            case SoundsType.Snow:
                {
                    m_WeatherSFX.Play();
                    break;
                }
            case SoundsType.Thunder:
                {
                    m_WeatherSFX.Play();
                    break;
                }
            case SoundsType.Wind:
                {
                    m_WeatherSFX.Play();
                    break;
                }
        }
    }

    public void StopSFX()
    {
        switch (m_Type)
        {
            case SoundsType.Rain:
                {
                    m_WeatherSFX.Stop();
                    break;
                }
            case SoundsType.Snow:
                {
                    m_WeatherSFX.Stop();
                    break;
                }
            case SoundsType.Thunder:
                {
                    m_WeatherSFX.Stop();
                    break;
                }
            case SoundsType.Wind:
                {
                    m_WeatherSFX.Stop();
                    break;
                }
        }
    }
}
