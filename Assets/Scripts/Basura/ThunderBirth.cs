using System;
using UnityEngine;

public class ThunderBirth : MonoBehaviour
{
    public ParticleSystem lightningParticleSystem;
    public ParticleSystemSubEmitterType m_Type;
    public static event Action Birthed;
    private float timer = 3f;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            if (m_Type == ParticleSystemSubEmitterType.Birth)
            {
                Birthed?.Invoke();
                timer = 5f;
                Debug.Log(m_Type.ToString());
            }
        }
    }
}