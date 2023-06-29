using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static Vector3 GetPositionZX(this Transform t)
    {
        var x = t.position;
        x.y = 0;
        return x;
    }
}

public static class VectorExtension
{
    public static Vector3 GetPositionZX(this Vector3 t)
    {
        var x = t;
        x.y = 0;
        return x;
    }
}

[RequireComponent(typeof(Vehicle))]
public class AIRaySensor : MonoBehaviour
{
    [SerializeField] private Transform[] m_Rays;
    [SerializeField] private float m_RaycastDistance;
    public float RaycastDistance => m_RaycastDistance;

    public (bool, float) Raycast()
    {
        float dist = -1;

        foreach(Transform v in m_Rays)
        {
            RaycastHit hit;

            if(Physics.Raycast(v.position, v.forward, out hit, m_RaycastDistance))
            {
                if(dist < 0 || hit.distance < dist)
                {
                    dist = hit.distance;
                }
            }
        }

        return(dist > 0, dist);
    }

    private void OnDrawGizmos()
    {
        foreach(Transform v in m_Rays)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(v.position,v.position + v.forward * m_RaycastDistance);
        }
    }
}
