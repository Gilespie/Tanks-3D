using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MatchMember
{
    [SerializeField] private Vehicle vehicle;

    public override void OnStartServer()
    {
        base.OnStartServer();

        m_TeamId = MatchController.GetTeamID();
        m_Nickname = "b_" + GetRandomName();

        data = new MatchMemberData((int)netId, m_Nickname, m_TeamId, netIdentity);

        transform.position = NetworkSessionManager.Instance.GetSpawnPointByTeam(m_TeamId);

        ActiveVehicle = vehicle;
        ActiveVehicle.TeamId = m_TeamId;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = m_Nickname;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        MatchMemberList.Instance.SvRemoveMember(data);
    }

    private void Start()
    {
        if(isServer == true)
        {
            MatchMemberList.Instance.SvAddMember(data);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        ActiveVehicle = vehicle;
        ActiveVehicle.TeamId = m_TeamId;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = m_Nickname;
    }

    private string GetRandomName()
    {
        string[] names =
        {
            "John",
            "Alice",
            "Lara",
            "Ada",
            "Clar",
            "Cris",
            "Sara",
            "Vesker",
            "Jill",
            "Barry",
            "Leon",
            "Johns",
            "Iona",
            "Tom",
            "James",
            "Da Vinchi",
            "Caesar",
            "Jack"
        };

        return names[Random.Range(0, names.Length)];
    }
}
