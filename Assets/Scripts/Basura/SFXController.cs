using UnityEngine;

public class SFXController : MonoBehaviour
{
    [SerializeField] private SoundsType[] m_SoundsType;
    [SerializeField] private AudioClip[] m_SFXThunder;
    private int randomIndex;
    private float randomPitch;

    void Start()
    {
        //ThunderBirth.Birthed += OnBirthed;
        InvokeRepeating("GetRandomVolume", 0f, 3f);
        InvokeRepeating("GetRandomIndex", 0, 3f);
        InvokeRepeating("GetRandomPitch", 0, 4f);
    }

    private float GetRandomIndex()
    {
        return randomIndex = Random.Range(0, m_SFXThunder.Length);
    }

    private float GetRandomPitch()
    {
        return randomPitch = Random.Range(0.8f, 1.3f);
    }

    private void PlaySFX(SoundsType typeSound)
    {
        if(typeSound == SoundsType.Rain)
        {

        }
    }

    /*public void OnBirthed()
    {
        m_AudioSources[2].pitch = randomPitch;
        m_AudioSources[2].PlayOneShot(m_SFXThunder[randomIndex]);
    }*/
}
