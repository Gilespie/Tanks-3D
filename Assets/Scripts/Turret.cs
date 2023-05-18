using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Turret : NetworkBehaviour
{
    public UnityAction<int> AmmoChanged;

    [SerializeField] private Transform m_LaunchPoint;
    public Transform LaunchPoint => m_LaunchPoint;

    [SerializeField] private Projectile m_Projectile;
    public Projectile Projectile => m_Projectile;


    [SyncVar, SerializeField] private int m_AmmoCount;
    public int AmmoCount => m_AmmoCount;

    [SerializeField] private float m_FireRate;

    private float fireTimer;
    public float fireRateNormalize => fireTimer / m_FireRate;

    [Server]
    public void SvAddAmmo(int count)
    {
        m_AmmoCount += count;
        RpcAmmoChange();
    }

    [Server]
    protected virtual bool SvDrawAmmo(int count)
    {
        if(m_AmmoCount == 0) return false;

        if(m_AmmoCount >= count)
        {
            m_AmmoCount -= count;
            RpcAmmoChange();
            return true;
        }

        return false;
    }

    protected virtual void OnFire() { }

    public void Fire()
    {
        if(hasAuthority == false) return;

        if (isClient == true)
        {
            CmdFire();
        }
    }

    [Command]
    private void CmdFire()
    {
        if (fireTimer > 0) return;

        if(SvDrawAmmo(1) == false) return;

        OnFire();

        fireTimer = m_FireRate;

        RpcFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if(isServer == true) return;

        fireTimer = m_FireRate;

        OnFire();
    }

    [ClientRpc]
    private void RpcAmmoChange()
    {
        AmmoChanged?.Invoke(m_AmmoCount);
    }

    protected virtual void Update()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }
}
