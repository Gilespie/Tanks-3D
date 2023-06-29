using Mirror;
using UnityEngine;

public class MatchMemberSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject m_BotPrefab;
    [SerializeField, Range(0, 15)] private int m_TargetAmountMemberTeam;

    [Server]
    public void SvRespawnVehiclesAllMembers()
    {
        SvRespawnPlayerVehicle();
        SvRespawnBotVehicle(); 
    }

    [Server]
    private void SvRespawnPlayerVehicle()
    {
        foreach (var p in FindObjectsOfType<Player>())
        {
            if (p.ActiveVehicle != null)
            {
                NetworkServer.UnSpawn(p.ActiveVehicle.gameObject);
                Destroy(p.ActiveVehicle.gameObject);
                p.ActiveVehicle = null;
            }
        }

        foreach (var p in FindObjectsOfType<Player>())
        {
            p.SvSpawnClientVehicle();
        }
    }

    [Server]
    private void SvRespawnBotVehicle() 
    {
        foreach(var b in FindObjectsOfType<Bot>())
        {
            NetworkServer.UnSpawn(b.gameObject);
            Destroy(b.gameObject);
        }

        int botAmount = m_TargetAmountMemberTeam * 2 - MatchMemberList.Instance.MemberDataCount;

        for(int i = 0; i < botAmount; i++)
        {
            GameObject b = Instantiate(m_BotPrefab);
            NetworkServer.Spawn(b);
        }
    }
}