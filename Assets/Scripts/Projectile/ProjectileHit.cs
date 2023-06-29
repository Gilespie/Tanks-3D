using UnityEngine;

public enum ProjectileHitType
{
    Penetration,
    Non_Penetration,
    Ricochet,
    ModulePenetration,
    ModuleNon_Penetration,
    Environment
}

public class ProjectileHitResult
{
    public ProjectileHitType Type;
    public float Damage;
    public Vector3 Point;
}

[RequireComponent(typeof(Projectile))]
public class ProjectileHit : MonoBehaviour
{
    private const float RAYADVANCE = 1.1f;

    private Projectile projectile; 
    private RaycastHit raycastHit;
    private Armor hitArmor;
    private bool isHit;

    public bool IsHit => isHit;
    public Armor HitArmor => hitArmor;
    public RaycastHit RaycastHit => raycastHit;

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }
    public void Check()
    {
        if (isHit == true) return;

        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, projectile.Properties.Speed * Time.deltaTime * RAYADVANCE))
        {
            Armor armor =  raycastHit.collider.GetComponent<Armor>();

            if (armor != null)
            {
                hitArmor = armor;
            }

            isHit = true;
        }
    }

    public ProjectileHitResult GetHitResult()
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();

        if(hitArmor == null)
        {
            hitResult.Type = ProjectileHitType.Environment;
            hitResult.Point = raycastHit.point;
            return hitResult;
        }

        float normalization = projectile.Properties.NormalizationAngle;

        if(projectile.Properties.Calibr > hitArmor.Thickness * 2)
        {
            normalization = (projectile.Properties.NormalizationAngle * 1.4f * projectile.Properties.Calibr) / hitArmor.Thickness;
        }

        float angle = Mathf.Abs(Vector3.SignedAngle(-projectile.transform.forward, raycastHit.normal, projectile.transform.right)) - normalization;
        float reduceArmor = hitArmor.Thickness / Mathf.Cos(angle * Mathf.Deg2Rad);
        float projectilePenetration = projectile.Properties.GetSpreadArmorPenetration();

        //Debugs
        Debug.DrawRay(raycastHit.point, -projectile.transform.forward, Color.red);
        Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.green);
        Debug.DrawRay(raycastHit.point, projectile.transform.right, Color.yellow);

        if(angle >= projectile.Properties.RicochetAngle && projectile.Properties.Calibr < hitArmor.Thickness * 3 && hitArmor.Type == ArmorType.Vehicle)
        {
            hitResult.Type = ProjectileHitType.Ricochet;
        }

        else if(projectilePenetration > reduceArmor)
        {
            hitResult.Type = ProjectileHitType.Penetration;
        }

        else if(projectilePenetration < reduceArmor)
        {
            hitResult.Type = ProjectileHitType.Non_Penetration;
        }

        //Debug.LogErrorFormat($"Armor: {hitArmor.Thickness}, reduceArmor: {reduceArmor}, angle: {angle}, norm: {normalization}, penetration: {projectilePenetration}, Type: {hitResult.Type}.");

        if(hitResult.Type == ProjectileHitType.Penetration)
            hitResult.Damage = projectile.Properties.GetDamageSpread();
        else
            hitResult.Damage = 0;


        if (hitArmor.Type == ArmorType.Module)
        {
            if (hitResult.Type == ProjectileHitType.Penetration)
                hitResult.Type = ProjectileHitType.ModulePenetration;

            if (hitResult.Type == ProjectileHitType.Non_Penetration)
                hitResult.Type = ProjectileHitType.ModuleNon_Penetration;
        }

        hitResult.Point = raycastHit.point;

        return hitResult;
    }
}
