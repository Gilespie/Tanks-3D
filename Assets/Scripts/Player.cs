using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    public UnityAction<Vehicle> VehicleSpawned;

    public static int m_TeamCounter;
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

    [Header("Player")] 
    [SyncVar(hook = nameof(OnNicknameChanged))] string Nickname;

    [SyncVar]
    [SerializeField] private int m_TeamId;
    public int TeamId => m_TeamId;
    
    private void OnNicknameChanged(string oldName, string newName)
    {
        gameObject.name = "Player_" + newName; //On Client
    }

    [Command] //On Server
    public void CmdSetName(string name)
    {
        Nickname = name;
        gameObject.name = "Player_" + name;
    }

    [Command]
    public void CmdSetTeamId(int teamId)
    {
        this.m_TeamId = teamId;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        m_TeamId = m_TeamCounter % 2;
        m_TeamCounter++;
    }

    public override void OnStartClient()
    {
        base.OnStartAuthority();

        if(hasAuthority == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);
        }
    }

    private void Update()
    {
        if(isLocalPlayer ==  true)
        {
            if(ActiveVehicle != null)
            {
                ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoom);
            }
        }

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

        playerVehicle.transform.position = TeamId % 2 == 0 ?
            NetworkSessionManager.Instance.RandomSpawnPointRed : 
            NetworkSessionManager.Instance.RandomSpawnPointYellow;

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

        if(ActiveVehicle != null && ActiveVehicle.hasAuthority && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }

        VehicleSpawned?.Invoke(ActiveVehicle);
    }
}
