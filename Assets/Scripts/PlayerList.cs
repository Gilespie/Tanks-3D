using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class PlayerList : NetworkBehaviour
{
    public static PlayerList Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<PlayerData> AllPlayerData = new List<PlayerData>();
    public static UnityAction<List<PlayerData>> UpdatePlayerList;

    public override void OnStartClient()
    {
        base.OnStartClient();

        AllPlayerData.Clear();
    }

    [Server]
    public void SvAddPlayer(PlayerData data)
    {
        AllPlayerData.Add(data);

        RpcClearPlayerDataList();

        for (int i = 0; i < AllPlayerData.Count; i++)
        {
            RpcAddPlayer(AllPlayerData[i]);
        }
    }

    [Server]
    public void SvRemovePlayer(PlayerData data)
    {
        for (int i = 0; i < AllPlayerData.Count; i++)
        {
            if (AllPlayerData[i].Id == data.Id)
            {
                AllPlayerData.RemoveAt(i);
                break;
            }
        }
        RpcRemovePlayer(data);
    }

    [ClientRpc]
    private void RpcClearPlayerDataList()
    {
        if (isServer == true) return;

        AllPlayerData.Clear();
    }

    [ClientRpc]
    private void RpcAddPlayer(PlayerData data)
    {
        if (isClient == true && isServer == true)
        {
            UpdatePlayerList?.Invoke(AllPlayerData);
            return;
        }

        AllPlayerData.Add(data);

        UpdatePlayerList?.Invoke(AllPlayerData);
    }

    [ClientRpc]
    private void RpcRemovePlayer(PlayerData data)
    {
        for (int i = 0; i < AllPlayerData.Count; i++)
        {
            if (AllPlayerData[i].Id == data.Id)
            {
                AllPlayerData.RemoveAt(i);
                break;
            }
        }
        UpdatePlayerList?.Invoke(AllPlayerData);
    }
}