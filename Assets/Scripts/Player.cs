using UnityEngine;
using Mirror;
using UnityEngine.Events;

[System.Serializable]
public class PlayerData
{
    public int Id;
    public string Nickname;
    public int TeamId;
    public PlayerData(int id, string nickname, int teamId)
    {
        Id = id;
        Nickname = nickname;
        TeamId = teamId;
    }
}

public static class PlayerDataReadWrite
{
    public static void WritePlayerData(this NetworkWriter writer, PlayerData value)
    {
        writer.WriteInt(value.Id);
        writer.WriteString(value.Nickname);
        writer.WriteInt(value.TeamId);
    }

    public static PlayerData ReadPlayerData(this NetworkReader reader)
    {
        return new PlayerData(reader.ReadInt(), reader.ReadString(), reader.ReadInt());
    }
}

public class Player : NetworkBehaviour
{
    public static int m_TeamCounter;
    public static UnityAction<int, int> ChangedFrag;

    public event UnityAction<Vehicle> VehicleSpawned;
    public event UnityAction<ProjectileHitResult> ProjectileHit;
    
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
    [SerializeField] private VehicleInputController m_VehicleInputController;

    public Vehicle ActiveVehicle { get; set; }

    [Header("Player")] 
    [SyncVar(hook = nameof(OnNicknameChanged))] public string Nickname;

    [SyncVar]
    [SerializeField] private int m_TeamId;
    public int TeamId => m_TeamId;

    private PlayerData data;
    public PlayerData Data => data;

    [SyncVar(hook = nameof(OnChangedFrag))]
    private int frag;
    public int Frag
    {
        get { return frag; }
        set
        { 
            frag = value;
            //Server
            ChangedFrag?.Invoke((int)netId, frag);
        }
    }

    [Server]
    public void SvInvokeProjectileHit(ProjectileHitResult hitResult)
    {
        ProjectileHit?.Invoke(hitResult);

        RpcInvokeProjectileHit(hitResult.Type, hitResult.Damage, hitResult.Point);
    }

    [ClientRpc]
    private void RpcInvokeProjectileHit(ProjectileHitType type, float damage, Vector3 hitPoint)
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();
        hitResult.Type = type;
        hitResult.Damage = damage;
        hitResult.Point = hitPoint;

        ProjectileHit?.Invoke(hitResult);
    }

    //Client
    private void OnChangedFrag(int old, int newValue)
    {
        ChangedFrag?.Invoke((int)netId, newValue);
    }
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

    public override void OnStopServer()
    {
        base.OnStopServer();

        PlayerList.Instance.SvRemovePlayer(data);
    }

    public override void OnStartClient()
    {
        //base.OnStartAuthority();
        base.OnStartClient();

        if(hasAuthority == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

            data = new PlayerData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, m_TeamId);

            CmdAddPlayer(Data);
            CmdUpdatePlayer(Data);
        }
    }

    [Command]
    private void CmdAddPlayer(PlayerData data)
    {
        PlayerList.Instance.SvAddPlayer(data);
    }

    [Command]
    private void CmdUpdatePlayer(PlayerData data)
    {
        this.data = data;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if(hasAuthority == true)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }   
    
    private void OnMatchEnd()
    {
        if(ActiveVehicle != null)
        {
            ActiveVehicle.SetTargetControl(Vector3.zero);
            m_VehicleInputController.enabled = false;
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
                NetworkSessionManager.Match.SvRestartMatch();
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
    public void SvSpawnClientVehicle()
    {
        if (ActiveVehicle != null) return;

        GameObject playerVehicle = Instantiate(m_VehiclePrefab.gameObject, transform.position, Quaternion.identity);

        playerVehicle.transform.position = TeamId % 2 == 0 ?
            NetworkSessionManager.Instance.RandomSpawnPointRed : 
            NetworkSessionManager.Instance.RandomSpawnPointYellow;

        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.TeamId = TeamId;

        RpcSetVehicle(ActiveVehicle.netIdentity, netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle, NetworkIdentity owner)
    {
        if (vehicle == null) return;

        ActiveVehicle = vehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.TeamId = TeamId;

        if (ActiveVehicle != null && ActiveVehicle.hasAuthority && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }

        m_VehicleInputController.enabled = true;

        VehicleSpawned?.Invoke(ActiveVehicle);
    }
}
