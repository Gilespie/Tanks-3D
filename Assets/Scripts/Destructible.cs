using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public UnityAction<int> HitPointChange;
    [SerializeField] private UnityEvent OnDestroyed;

    [SerializeField] private int m_HitPointMax;
    public int HitPointMax => m_HitPointMax;
    
    [SerializeField] private GameObject m_DestroySFX;

    private int currentHealth;
    public int HitPoint => currentHealth;

    [SyncVar(hook = nameof(ChangeHitPoint))]
    private int syncCurrentHealth;

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

        if(syncCurrentHealth <= 0)
        {
            if(m_DestroySFX != null)
            {
                GameObject sfx = Instantiate(m_DestroySFX, transform.position, Quaternion.identity);
                NetworkServer.Spawn(sfx);
            }

            syncCurrentHealth = 0;

            RpcDestroy();
        }
    }

    [ClientRpc]
    private void RpcDestroy()
    {
        OnDestructibleDestroy();
    }

    protected virtual void OnDestructibleDestroy()
    {
        OnDestroyed?.Invoke();
    }

    private void ChangeHitPoint(int oldValue, int newValue)
    {
        currentHealth = newValue;
        HitPointChange?.Invoke(newValue);
    }

    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue) 
    {
        
    }
}
