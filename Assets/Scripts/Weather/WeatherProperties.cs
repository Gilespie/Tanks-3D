using UnityEngine;

[CreateAssetMenu(fileName = "WeatherProp", menuName = "ScriptableObject/Weather", order = 0)]
public class WeatherProperties : ScriptableObject
{
    [Header("Sounds")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField, Range(0f, 1f)] private float m_Volume;
    [SerializeField] private AudioClip[] m_SFXs;

    [Header("Weathers prefabs")]
    [SerializeField] private ParticleSystem m_Mainprefab;
    [SerializeField] private ParticleSystem[] m_Particles;

    [Header("")]

    [Header("Wind")]
    [SerializeField] private WindZone m_WindZone;
    [SerializeField, Range(0f, 1f)] private float m_PowerOfWind;
    [SerializeField] private Vector3 m_Direction;
    [SerializeField] private float randomDirection;
    
    public Vector3 GetRandomWindDirection()
    {
        randomDirection = Random.Range(0f, 360f);
        return m_Direction = new Vector3(0, randomDirection, 0);
    }



    
}
