using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Player : MatchMember
{    
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

    private void Start()
    {
        m_VehicleInputController.enabled = false;
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

    public override void OnStartServer()
    {
        base.OnStartServer();

        m_TeamId = MatchController.GetTeamID();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        MatchMemberList.Instance.SvRemoveMember(data);
    }

    public override void OnStartClient()
    {
        //base.OnStartAuthority();
        base.OnStartClient();

        if(isOwned == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
            NetworkSessionManager.Match.MatchStart += OnMatchStart;

            data = new MatchMemberData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, m_TeamId, netIdentity);

            CmdAddPlayer(Data);
            CmdUpdatePlayer(Data);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isOwned == true)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }

    private void OnMatchStart()
    {
        m_VehicleInputController.enabled = true;
    }

    private void OnMatchEnd()
    {
        if (ActiveVehicle != null)
        {
            ActiveVehicle.SetTargetControl(Vector3.zero);
            m_VehicleInputController.enabled = false;
        }
    }

    [Command]
    private void CmdAddPlayer(MatchMemberData data)
    {
        MatchMemberList.Instance.SvAddMember(data);
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

        if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }

        m_VehicleInputController.enabled = false;

        VehicleSpawned?.Invoke(ActiveVehicle);
    }
}
