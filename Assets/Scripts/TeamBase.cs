using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBase : MonoBehaviour
{
    [SerializeField] private float m_CaptureLevel;
    public float CaptureLevel => m_CaptureLevel;

    [SerializeField] private float m_CaptureAmountPerVehicle;
    [SerializeField] private int m_TeamId;
    [SerializeField] private List<Vehicle> m_AllVehicles = new List<Vehicle>();

    private void OnTriggerEnter(Collider other)
    {
        Vehicle v = other.transform.root.GetComponent<Vehicle>();

        if (v == null) return;
        if (v.HitPoint == 0) return;
        if (m_AllVehicles.Contains(v) == true) return;
        if (v.Owner.GetComponent<Player>().TeamId == m_TeamId) return;

        v.HitPointChange += OnHitPointChange;

        m_AllVehicles.Add(v);
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle v = other.transform.root.GetComponent<Vehicle>();

        if (v == null) return;

        v.HitPointChange -= OnHitPointChange;

        m_AllVehicles.Remove(v);
    }

    private void OnHitPointChange(int hitpoint)
    {
        m_CaptureLevel = 0;
    }

    private void Update()
    {
        if (NetworkSessionManager.Instance.IsServer == true)
        {
            bool isAllDead = true;

            for (int i = 0; i < m_AllVehicles.Count; i++)
            {
                if (m_AllVehicles[i].HitPoint != 0)
                {
                    isAllDead = false;

                    m_CaptureLevel += m_CaptureAmountPerVehicle * Time.deltaTime;
                    m_CaptureLevel = Mathf.Clamp(m_CaptureLevel, 0f, 100f);
                }
            }

            if (m_AllVehicles.Count == 0 || isAllDead == true)
            {
                m_CaptureLevel = 0;
            }
        }
    }

    public void Reset()
    {
        m_CaptureLevel = 0;

        for (int i = 0; i < m_AllVehicles.Count; i++)
        {
            m_AllVehicles[i].HitPointChange -= OnHitPointChange;
        }

        m_AllVehicles.Clear();
    }
}
