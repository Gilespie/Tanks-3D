using TMPro;
using UnityEngine;

public class UIAmmoText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_AmmoText;
    private Turret turret;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if (turret != null) turret.AmmoChanged -= OnAmmoChanged;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        turret = vehicle.Turret;

        turret.AmmoChanged += OnAmmoChanged;
        m_AmmoText.SetText(turret.AmmoChanged.ToString());
    }

    private void OnAmmoChanged(int ammo)
    {
        m_AmmoText.SetText(Player.Local.ActiveVehicle.Turret.AmmoCount.ToString());
    }
}
