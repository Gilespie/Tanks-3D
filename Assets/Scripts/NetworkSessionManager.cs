using UnityEngine;
using Mirror;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] m_SpawnZoneRed;
    [SerializeField] private SphereArea[] m_SpawnZoneGreen;
    [SerializeField] private GameEventCollector m_GameEventCollector;
    [SerializeField] private MatchController m_MatchController;

    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
    public static GameEventCollector Events => Instance.m_GameEventCollector;
    public static MatchController Match => Instance.m_MatchController;
    public bool IsServer => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly;
    public bool IsClient => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly;

    public Vector3 RandomSpawnPointRed => m_SpawnZoneRed[Random.Range(0, m_SpawnZoneRed.Length)].RandomInside;
    public Vector3 RandomSpawnPointGreen => m_SpawnZoneGreen[Random.Range(0, m_SpawnZoneRed.Length)].RandomInside;

    public Vector3 GetSpawnPointByTeam(int teamId)
    {
        return teamId % 2 == 0 ? RandomSpawnPointRed : RandomSpawnPointGreen;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        m_GameEventCollector.SvOnAddPlayer();
    }
}
