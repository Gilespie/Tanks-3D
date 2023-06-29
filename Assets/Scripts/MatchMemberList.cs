using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class MatchMemberList : NetworkBehaviour
{
    public static MatchMemberList Instance;
    public static UnityAction<List<MatchMemberData>> UpdateList;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private List<MatchMemberData> allMemberData = new List<MatchMemberData>();
    public int MemberDataCount => allMemberData.Count;

    public override void OnStartClient()
    {
        base.OnStartClient();

        allMemberData.Clear();
    }

    [Server]
    public void SvAddMember(MatchMemberData data)
    {
        allMemberData.Add(data);

        RpcClearMemberDataList();

        for (int i = 0; i < allMemberData.Count; i++)
        {
            RpcAddMember(allMemberData[i]);
        }
    }

    [Server]
    public void SvRemoveMember(MatchMemberData data)
    {
        for (int i = 0; i < allMemberData.Count; i++)
        {
            if (allMemberData[i].Id == data.Id)
            {
                allMemberData.RemoveAt(i);
                break;
            }
        }
        RpcRemoveMember(data);
    }

    [ClientRpc]
    private void RpcClearMemberDataList()
    {
        if (isServer == true) return;

        allMemberData.Clear();
    }

    [ClientRpc]
    private void RpcAddMember(MatchMemberData data)
    {
        if (isClient == true && isServer == true)
        {
            UpdateList?.Invoke(allMemberData);
            return;
        }

        allMemberData.Add(data);

        UpdateList?.Invoke(allMemberData);
    }

    [ClientRpc]
    private void RpcRemoveMember(MatchMemberData data)
    {
        for (int i = 0; i < allMemberData.Count; i++)
        {
            if (allMemberData[i].Id == data.Id)
            {
                allMemberData.RemoveAt(i);
                break;
            }
        }
        UpdateList?.Invoke(allMemberData);
    }
}