using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Destructible : NetworkBehaviour
{
    public UnityAction<int> HitPointChange;
    [SerializeField] private UnityEvent<Destructible> Destroyed;
    public UnityEvent<Destructible> OnEventDeath => Destroyed;

    [SerializeField] private int m_HitPointMax;
    public int HitPointMax => m_HitPointMax;
    
    [SerializeField] private GameObject m_DestroySFX;

    private int currentHealth;
    public int HitPoint => currentHealth;

    [SyncVar(hook = nameof(ChangeHitPoint))]
    private int syncCurrentHealth;

    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;

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
        Destroyed?.Invoke(this);
    }

    private void ChangeHitPoint(int oldValue, int newValue)
    {
        currentHealth = newValue;
        HitPointChange?.Invoke(newValue);
    }

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue) 
    {
        
    }
}
