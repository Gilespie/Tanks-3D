using UnityEngine;

public enum ProjectileType
{
    ArmorPiercing,
    HighExplosive,
    Subcaliber
}

[CreateAssetMenu(fileName = "ProjectileProp", menuName = "ScriptableObject/Projectile")]
public class ProjectileProperties : ScriptableObject
{
    [SerializeField] private ProjectileType m_Type;

    [Header("Common")]
    [SerializeField] private Projectile m_ProjectilePrefab;
    [SerializeField] private Sprite m_ProjectileIcon;

    [Header("Movement")]
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_Mass;
    [SerializeField] private float m_ImpactForce;

    [Header("Damage")]
    [SerializeField] private float m_Damage;
    [SerializeField, Range(0f, 1f)] private float m_DamageSpread;

    [Header("Calibr")]
    [SerializeField] private float m_Calibr;

    [Header("Armor Penetration")]
    [SerializeField] private float m_ArmorPenetration;
    [SerializeField, Range(0f, 1f)] private float m_ArmorPenetrationSpred;
    [SerializeField, Range(0f, 90f)] private float m_NormalizationAngle;
    [SerializeField, Range(0f, 90f)] private float m_RicochetAngle;

    public ProjectileType Type => m_Type;
    public Projectile ProjectilePrefab => m_ProjectilePrefab;
    public Sprite ProjectileIcon => m_ProjectileIcon;
    public float Speed => m_Speed;
    public float Mass => m_Mass;
    public float ImpactForce => m_ImpactForce;
    public float Damage => m_Damage;
    public float DamageSpread => m_DamageSpread;
    public float Calibr => m_Calibr;
    
    public float ArmorPenetration => m_ArmorPenetration;
    public float ArmorPenetrationSpred => m_ArmorPenetrationSpred;
    public float NormalizationAngle => m_NormalizationAngle;
    public float RicochetAngle => m_RicochetAngle;

    public float GetDamageSpread()
    {
        return m_Damage * Random.Range(1 - m_DamageSpread, 1 + m_DamageSpread);
    }

    public float GetSpreadArmorPenetration()
    {
        return m_ArmorPenetration * Random.Range(1 - m_ArmorPenetrationSpred, 1 + m_ArmorPenetrationSpred);
    }
}
