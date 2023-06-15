using UnityEngine;
using Mirror;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileProperties m_ProjectileProperties;
    [SerializeField] private ProjectileMovement m_ProjectileMovement;
    [SerializeField] private ProjectileHit m_ProjectileHit;


    [SerializeField, Space(5)] private GameObject m_VisualModel;
    [SerializeField, Space(5)] private float m_DelayBeforDestroy;
    [SerializeField] private float m_LifeTime;

    public NetworkIdentity Owner { get; set; }
    public ProjectileProperties Properties => m_ProjectileProperties;
    
    private void Start()
    {
        Destroy(gameObject, m_LifeTime);
    }

    private void Update()
    {
        m_ProjectileHit.Check();
        m_ProjectileMovement.Move();

        if(m_ProjectileHit.IsHit == true)
        {
            OnHit();
        }
    }

    private void OnHit()
    {
        transform.position = m_ProjectileHit.RaycastHit.point;

        if (NetworkSessionManager.Instance.IsServer == true)
        {
            ProjectileHitResult hitResult = m_ProjectileHit.GetHitResult();

            if (hitResult.Type == ProjectileHitType.Penetration || hitResult.Type == ProjectileHitType.ModulePenetration)
            {
                if (NetworkSessionManager.Instance.IsServer == true)
                {
                    SvTakeDamage(hitResult);
                    SvAddFrag();
                }
            }

            Owner.GetComponent<Player>().SvInvokeProjectileHit(hitResult);
        }

        Destroy();
    }

    public void SvTakeDamage(ProjectileHitResult hitResult)
    {
        m_ProjectileHit.HitArmor.Destructible.SvApplyDamage((int)hitResult.Damage);
    }

    public void SvAddFrag()
    {
        if(m_ProjectileHit.HitArmor.Type == ArmorType.Module) return;

        if (m_ProjectileHit.HitArmor.Destructible.HitPoint <= 0)
        {
            if (Owner != null)
            {
                Player player = Owner.GetComponent<Player>();

                if (player != null)
                {
                    player.Frag++;
                }
            }
        }
    }

    private void Destroy()
    {
        m_VisualModel.SetActive(false);
        enabled = false;

        Destroy(gameObject, m_DelayBeforDestroy);
    }

    public void SetProperties(ProjectileProperties properties)
    {
        this.m_ProjectileProperties = properties;
    }
}
