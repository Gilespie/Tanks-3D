using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class GameEventCollector : NetworkBehaviour
{
    public UnityAction<Vehicle> PlayerVehicleSpawned;

    [Server]
    public void SvAddPlayer()
    {
        RpcAddPlayer();
    }

    [ClientRpc]
    public void RpcAddPlayer()
    {
        Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        PlayerVehicleSpawned(vehicle);
    }
}
