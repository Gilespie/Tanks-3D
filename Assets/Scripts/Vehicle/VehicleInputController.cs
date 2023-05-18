using UnityEngine;

[RequireComponent(typeof(Player))]
public class VehicleInputController : MonoBehaviour
{
    public const int m_AimDistance = 1000;
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>(); 
    }

    protected virtual void Update()
    {
        if (player == null) return;
        if (player.ActiveVehicle == null) return;

        if (player.hasAuthority && player.isLocalPlayer)
        {
            player.ActiveVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
            player.ActiveVehicle.NetAimPoint = TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position, VehicleCamera.Instance.transform.forward);

            if (Input.GetMouseButtonDown(0))
            {
                player.ActiveVehicle.Fire();
            }
        }

        
    }

    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 dirrection)
    {
        Ray ray = new Ray(start, dirrection);

        RaycastHit[] hits = Physics.RaycastAll(ray, m_AimDistance);

        var m = Player.Local.ActiveVehicle.GetComponent<Rigidbody>();

        foreach (var hit in hits)
        {
            if (hit.collider == m) continue;
            
            return hit.point;
        }
        return ray.GetPoint(m_AimDistance);
    }
}