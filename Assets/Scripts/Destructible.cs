using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public event UnityAction<int> HitPointChanged;
    public event UnityAction<Destructible> Destroyed;
    public event UnityAction<Destructible> Recovered;

    [SerializeField] private int m_HitPointMax;
    [SerializeField] private UnityEvent m_EventDestroyed;
    [SerializeField] private UnityEvent m_EventRecovered;

    [SerializeField] private int currentHealth;

    public int HitPointMax => m_HitPointMax;
    public int HitPoint => currentHealth;

    [SyncVar(hook = nameof(SyncHitPoint))]
    private int syncCurrentHealth;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHealth = m_HitPointMax;
        currentHealth = m_HitPointMax;
    }

    [Server]

    public void SvApplyDamage(int damage)
    {
        syncCurrentHealth -= damage;

        if (syncCurrentHealth <= 0)
        {
            syncCurrentHealth = 0;

            RpcDestroy();
        }
    }

    [Server]
    public void SvRecovery()
    {
        syncCurrentHealth = m_HitPointMax;
        currentHealth = m_HitPointMax;

        RpcRecovery();
    }
    #endregion

    #region Client

    private void SyncHitPoint(int oldValue, int newValue)
    {
        currentHealth = newValue;
        HitPointChanged?.Invoke(newValue);
    }

    [ClientRpc]
    private void RpcDestroy()
    {
        Destroyed?.Invoke(this);
        m_EventDestroyed?.Invoke();
    }

    [ClientRpc]
    private void RpcRecovery()
    {
        Recovered?.Invoke(this);
        m_EventRecovered?.Invoke();
    }
    #endregion
}
