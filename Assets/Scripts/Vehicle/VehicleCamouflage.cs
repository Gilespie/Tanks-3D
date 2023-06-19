using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleCamouflage : MonoBehaviour
{
    [SerializeField] private float m_BaseDistance;
    [SerializeField, Range(0f, 1f)] private float m_Percent;
    [SerializeField] private float m_PercentLerpRate;
    [SerializeField] private float m_PercentOfFire;
    private Vehicle vehicle;
    private float targetPercent;

    private float currentDistance;
    public float CurrentDistance => currentDistance;

    private void Start()
    {
        if (NetworkSessionManager.Instance.IsServer == false) return;

        vehicle = GetComponent<Vehicle>();
        vehicle.Turret.Shooted += OnShooted;

        targetPercent = 1f;
    }

    private void OnDestroy()
    {
        if(NetworkSessionManager.Instance.IsServer == false) return;

        vehicle.Turret.Shooted -= OnShooted;
    }

    private void Update()
    {
        if(NetworkSessionManager.Instance.IsServer == false) return;

        if (vehicle.NormalizedLinearVelocity > 0.01f)
            targetPercent = 0.5f;

        if (vehicle.NormalizedLinearVelocity <= 0.01f)
            targetPercent = 1.0f;

        m_Percent = Mathf.MoveTowards(m_Percent, targetPercent, Time.deltaTime * m_PercentLerpRate);
        m_Percent = Mathf.Clamp01(m_Percent);

        currentDistance = m_BaseDistance * m_Percent;
    }

    private void OnShooted()
    {
        m_Percent = m_PercentOfFire;
    }
}