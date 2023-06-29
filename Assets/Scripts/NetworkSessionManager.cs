using UnityEngine;
using Mirror;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] m_SpanZoneRed;
    [SerializeField] private SphereArea[] m_SpanZoneYellow;
    [SerializeField] private GameEventCollector m_GameEventCollector;
    [SerializeField] private MatchController m_MatchController;

    public Vector3 RandomSpawnPointRed => m_SpanZoneRed[UnityEngine.Random.Range(0, m_SpanZoneRed.Length)].RandomInside;
    public Vector3 RandomSpawnPointYellow => m_SpanZoneYellow[UnityEngine.Random.Range(0, m_SpanZoneRed.Length)].RandomInside;
    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
    public static GameEventCollector Events => Instance.m_GameEventCollector;
    public static MatchController Match => Instance.m_MatchController;
    public bool IsServer => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly;
    public bool IsClient => mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly;

    public Vector3 GetSpawnPointByTeam(int teamId)
    {
        return teamId % 2 == 0 ? RandomSpawnPointRed : RandomSpawnPointYellow;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        m_GameEventCollector.SvAddPlayer();
    }
}
