using UnityEngine;
using Mirror;

public class Vehicle : Destructible
{
    [SerializeField] protected float m_MaxLinearVelocity;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource m_EngineSFX;
    [SerializeField] private float m_EnginePitchModifier;

    [Header("Vehicle")]
    [SerializeField] protected Transform zoomOpticsPosition;
    public Transform ZoomOpticPosition => zoomOpticsPosition;

    public Turret Turret;
    public VehicleViewer Viewer;
    public virtual float LinearVelocity => 0;
    [SyncVar] private Vector3 m_NetAimPoint;
    protected Vector3 targetInputController;

    public int TeamId;

    protected float syncLinearVelocity;
    public float NormalizedLinearVelocity
    {
        get
        {
            if (Mathf.Approximately(0, syncLinearVelocity) == true) return 0;

            return Mathf.Clamp01(syncLinearVelocity / m_MaxLinearVelocity);
        }
    }

    public Vector3 NetAimPoint
    {
        get => m_NetAimPoint;

        set
        {
            m_NetAimPoint = value; //Client

            if(isOwned == true)
                CmdSetNetAimPoint(value); //Server

        }
    }

    [Command]
    private void CmdSetNetAimPoint(Vector3 v)
    {
        m_NetAimPoint = v;
    }

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

    public void Fire()
    {
        Turret.Fire();
    }

    public void SetVisible(bool visible)
    {
        if (visible == true)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Default")) return;

            SetLayerToAll("Default");
        }
        else
        {
            if(gameObject.layer == LayerMask.NameToLayer("IgnoreMainCamera")) return;

            SetLayerToAll("IgnoreMainCamera");
        }
    }

    private void SetLayerToAll(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach(Transform t in transform.GetComponentsInChildren<Transform>()) 
        {
            t.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
    {

    }
}