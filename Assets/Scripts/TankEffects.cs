using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackTank))]
public class TankEffects : MonoBehaviour
{
    private TrackTank tank;
    [SerializeField] private ParticleSystem[] exhausts;
    [SerializeField] private Vector2 minMaxExhaustEmission;

    [SerializeField] private ParticleSystem[] exhaustAtStart;
    private bool IsStopped;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }

    private void Update()
    {
        float exhaustEmission = Mathf.Lerp(minMaxExhaustEmission.x, minMaxExhaustEmission.y, tank.NormalizedLinearVelocity);
        
        for(int i = 0; i < exhausts.Length; i++)
        {
            ParticleSystem.EmissionModule emission = exhausts[i].emission;
            emission.rateOverTime = exhaustEmission;
        }

        if(tank.LinearVelocity < 0.1f)
        {
            IsStopped = true;
        }

        if(tank.LinearVelocity > 0.1f)
        {
            if(IsStopped == true)
            {
                for(int i = 0; i < exhaustAtStart.Length; i++)
                {
                    exhaustAtStart[i].Play();
                }
            }

            IsStopped = false;
        }
    }
}
