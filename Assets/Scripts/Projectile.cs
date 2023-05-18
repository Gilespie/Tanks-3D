using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject m_VisualModel;
    [SerializeField] private float m_LifeTime;
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_Mass;
    [SerializeField] private float m_Damage;
    [SerializeField, Range(0f, 1f)] private float m_DamageScatter;
    [SerializeField] private float m_ImpactForce;

    private const float RayAdvance = 1.1f;

    private void Start()
    {
        Destroy(gameObject, m_LifeTime);
    }

    private void UpdateProjectile()
    {
        transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * m_Mass)).normalized;

        Vector3 step = transform.forward * m_Speed * Time.deltaTime;

        RaycastHit hit;

        //Raycast hit effect
        if(Physics.Raycast(transform.position, transform.forward, out hit,  m_Speed * Time.deltaTime * RayAdvance))
        {
            transform.position = hit.point;

            var destructible = hit.transform.root.GetComponent<Destructible>();

            if( destructible)
            {
                //is your projectile
                //if yes send command to server

                if(NetworkSessionManager.Instance.IsServer)
                {
                    float dmg = m_Damage + Random.Range(-m_DamageScatter, m_DamageScatter) * m_Damage;

                    destructible.SvApplyDamage((int)m_Damage);
                }
            }

            OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);

            return;
        }

        transform.position += step;
    }

    private void OnProjectileLifeEnd(Collider col, Vector3 pos, Vector3 normal)
    {
        m_VisualModel.SetActive(false);
        enabled = false;
    }

    private void Update()
    {
        UpdateProjectile();
    }
}
