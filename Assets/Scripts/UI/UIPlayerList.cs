using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerList : MonoBehaviour
{
    [SerializeField] private Transform m_LocalTeamPanel;
    [SerializeField] private Transform m_OtherTeamPanel;

    [SerializeField] private UIPlayerLabel m_PlayerLabelPrefab;

    private List<UIPlayerLabel> allPlayerLable = new List<UIPlayerLabel>();

    private void Start()
    {
        PlayerList.UpdatePlayerList += OnUpdatePlayerList;
        Player.ChangedFrag += OnChangeFrag;
    }

    private void OnDestroy()
    {
        PlayerList.UpdatePlayerList -= OnUpdatePlayerList;
        Player.ChangedFrag -= OnChangeFrag;
    }

    private void OnChangeFrag(int playerNetId, int frag)
    {
        for (int i = 0; i < allPlayerLable.Count; i++)
        {
            if (allPlayerLable[i].NetId == playerNetId)
            {
                allPlayerLable[i].UpdateFrags(frag);
            }
        }
    }

    private void OnUpdatePlayerList(List<PlayerData> playerData)
    {
        for(int i = 0; i < m_LocalTeamPanel.childCount; i++)
        {
            Destroy(m_LocalTeamPanel.GetChild(i).gameObject);
        }

        for(int i = 0; i <m_OtherTeamPanel.childCount; i++)
        {
            Destroy(m_OtherTeamPanel.GetChild(i).gameObject);
        }

        allPlayerLable.Clear();

        for(int i = 0; i < playerData.Count; i++)
        {
            if (playerData[i].TeamId == Player.Local.TeamId)
            AddPlayerLabel(playerData[i], m_PlayerLabelPrefab, m_LocalTeamPanel);

            if (playerData[i].TeamId != Player.Local.TeamId)
                AddPlayerLabel(playerData[i], m_PlayerLabelPrefab, m_OtherTeamPanel);
        }
    }

    private void AddPlayerLabel(PlayerData data, UIPlayerLabel label, Transform parent)
    {
        UIPlayerLabel uiPlayerLbel = Instantiate(m_PlayerLabelPrefab);
        uiPlayerLbel.transform.SetParent(parent);
        uiPlayerLbel.Init(data.Id, data.Nickname);

        allPlayerLable.Add(uiPlayerLbel);
    }
}
