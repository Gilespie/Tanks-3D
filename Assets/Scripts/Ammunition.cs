using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Ammunition : NetworkBehaviour
{
    public event UnityAction<int> AmmoCountChanged;
    [SerializeField] private ProjectileProperties m_ProjectileProp;

    [SyncVar(hook = nameof(SyncAmmoCount))]
    [SerializeField] private int m_SyncAmmoCount;

    public ProjectileProperties ProjectileProp => m_ProjectileProp;
    public int AmmoCount => m_SyncAmmoCount;

    #region Server
    [Server]
    public void SvAmmoCount(int count)
    {
        m_SyncAmmoCount += count;
    }

    [Server]
    public bool SvDrawAmmo(int count) 
    {
        if (m_SyncAmmoCount == 0) return false;

        if(m_SyncAmmoCount >= count)
        {
            m_SyncAmmoCount -= count;
            return true;
        }

        return false;
    }
    #endregion

    #region Client
    private void SyncAmmoCount(int oldValue, int newValue)
    {
        AmmoCountChanged?.Invoke(newValue);
    }
    #endregion
}
