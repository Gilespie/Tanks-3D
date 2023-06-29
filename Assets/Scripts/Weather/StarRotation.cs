using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRotation : MonoBehaviour
{
    [SerializeField] private float m_SpeedRotation;
    //private ParticleSystem m_Particle; // TODO: En el futuro
    private float dir;
    private Quaternion defaultRotation;
    private float maxAngle = 360f;

    private void Start()
    {
        defaultRotation = transform.rotation;
    }

    void Update()
    {
        dir += m_SpeedRotation * Time.deltaTime;
        transform.rotation = Quaternion.Euler(dir, 0,0);

        if (transform.rotation.x >= maxAngle)
        {
            transform.rotation = defaultRotation;
            return;
        }
    }
}
