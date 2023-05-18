using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using System;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_HealthText;
    private Destructible destructible;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if (destructible != null) destructible.HitPointChange -= OnHitPointChange;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        destructible = vehicle;

        destructible.HitPointChange += OnHitPointChange;
        m_HealthText.SetText(destructible.HitPoint.ToString());
    }

    private void OnHitPointChange(int arg0)
    {
        m_HealthText.SetText(Player.Local.ActiveVehicle.HitPoint.ToString());
    }
}
