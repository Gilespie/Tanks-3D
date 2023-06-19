using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Turret : NetworkBehaviour
{
    public event UnityAction<int> UpdateSelectedAmmunition;
    public event UnityAction Shooted;

    [SerializeField] private Transform m_LaunchPoint;
    public Transform LaunchPoint => m_LaunchPoint;

    [SerializeField] private float m_FireRate;

    [SerializeField] protected Ammunition[] m_Amminution;
    public Ammunition[] Ammunition => m_Amminution;

    [SyncVar] private int syncSelectedAmmunition;

    private float fireTimer;

    public int SelectedAmmunitionIndex => syncSelectedAmmunition;
    public ProjectileProperties SelectedProjectile => m_Amminution[syncSelectedAmmunition].ProjectileProp;
    public float fireRateNormalize => fireTimer / m_FireRate;

    protected virtual void OnFire() { }
    
    public void SetSelectProjectile(int index)
    {
        if (hasAuthority == false) return;

        if(index < 0 || index > m_Amminution.Length) return;

        syncSelectedAmmunition = index;

        if(isClient ==  true)
        {
            CmdReloadAmmunition();
        }

        UpdateSelectedAmmunition?.Invoke(index);
    }

    public void Fire()
    {
        if(hasAuthority == false) return;

        if (isClient == true)
        {
            CmdFire();
        }
    }

    [Command]
    private void CmdReloadAmmunition()
    {
        fireTimer = m_FireRate;
    }

    [Command]
    private void CmdFire()
    {
        if (fireTimer > 0) return;

        if (m_Amminution[syncSelectedAmmunition].SvDrawAmmo(1) == false) return;

        OnFire();

        fireTimer = m_FireRate;

        RpcFire();

        Shooted?.Invoke();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if(isServer == true) return;

        fireTimer = m_FireRate;

        OnFire();

        Shooted?.Invoke();
    }

    protected virtual void Update()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }
}
