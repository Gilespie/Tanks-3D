using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] m_SpanZoneRed;
    [SerializeField] private SphereArea[] m_SpanZoneYellow;
    [SerializeField] private GameEventCollector m_GameEventCollector;
    public Vector3 RandomSpawnPointRed => m_SpanZoneRed[UnityEngine.Random.Range(0, m_SpanZoneRed.Length)].RandomInside;
    public Vector3 RandomSpawnPointYellow => m_SpanZoneYellow[UnityEngine.Random.Range(0, m_SpanZoneRed.Length)].RandomInside;
    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
    public static GameEventCollector Events => Instance.m_GameEventCollector;
    public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
    public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        m_GameEventCollector.SvAddPlayer();
    }
}
