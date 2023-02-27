using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public static Player Local
    {
        get
        {
            var x = NetworkClient.localPlayer;

            if (x != null) return x.GetComponent<Player>();

            return null;
        }
    }
    [SerializeField] private Vehicle m_VehiclePrefab;

    public Vehicle ActiveVehicle { get; set; }

    private void Update()
    {
        if(isServer == true)
        {
            if(Input.GetKeyDown(KeyCode.F9))
            {
                foreach(var p in FindObjectsOfType<Player>())
                {
                    if(p.ActiveVehicle != null)
                    {
                        NetworkServer.UnSpawn(p.ActiveVehicle.gameObject);
                        Destroy(p.ActiveVehicle.gameObject);

                        p.ActiveVehicle = null;
                    }
                }

                foreach(var p in FindObjectsOfType<Player>())
                {
                    p.SvSpawnClientVehicle();
                }
            }
        }

        if(isOwned == true)
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    [Server]
    private void SvSpawnClientVehicle()
    {
        if (ActiveVehicle != null) return;

        GameObject playerVehicle = Instantiate(m_VehiclePrefab.gameObject, transform.position, Quaternion.identity);
        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;

        RpcSetVehicle(ActiveVehicle.netIdentity, netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle, NetworkIdentity owner)
    {
        if (vehicle == null) return;

        ActiveVehicle = vehicle.GetComponent<Vehicle>();

        if(ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }
    }
}
