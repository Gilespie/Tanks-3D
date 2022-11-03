using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] protected float m_MaxLinearVelocity;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource m_EngineSFX;
    [SerializeField] private float m_EnginePitchModifier;

    [Header("Vehicle")]
    [SerializeField] protected Transform zoomOpticsPosition;
    public Transform ZoomOpticPosition => zoomOpticsPosition;

    public virtual float LinearVelocity => 0;

    public float NormalizedLinearVelocity
    {
        get
        {
            if (Mathf.Approximately(0, LinearVelocity) == true) return 0;

            return Mathf.Clamp01(LinearVelocity / m_MaxLinearVelocity);
        }
    }

    protected Vector3 targetInputController;


    public void SetTargetControl(Vector3 control)
    {
        targetInputController = control.normalized;
    }

    protected virtual void Update()
    {
        UpdateEngineSFX();
    }

    private void UpdateEngineSFX()
    {
        if (m_EngineSFX != null)
        {
            m_EngineSFX.pitch = 1.0f + m_EnginePitchModifier * NormalizedLinearVelocity;
            m_EngineSFX.volume = 0.5f + NormalizedLinearVelocity;
        }
    }
}