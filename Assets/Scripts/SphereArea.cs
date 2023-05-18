using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereArea : MonoBehaviour
{
    [SerializeField] private float m_Raduis;
    [SerializeField] private Color m_Color = Color.blue;

    public Vector3 RandomInside
    {
        get
        {
            var pos = UnityEngine.Random.insideUnitSphere * m_Raduis + transform.position;

            pos.y = transform.position.y;

            return pos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = m_Color;
        Gizmos.DrawSphere(transform.position, m_Raduis);
    }
}
