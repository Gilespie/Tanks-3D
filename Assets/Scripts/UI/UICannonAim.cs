using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image m_Aim;
    [SerializeField] private Image m_ReloadAim;
    private Vector3 aimPosition;

    void Update()
    {
        if (Player.Local == null) return;
        if (Player.Local.ActiveVehicle == null) return;

        Vehicle vehicle = Player.Local.ActiveVehicle;

        m_ReloadAim.fillAmount = vehicle.Turret.fireRateNormalize;

        aimPosition = VehicleInputController.TraceAimPointWithoutPlayerVehicle(vehicle.Turret.LaunchPoint.position, vehicle.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

        if(result.z > 0)
        {
            result.z = 0;

            m_Aim.transform.position = result;
        }
    }
}
