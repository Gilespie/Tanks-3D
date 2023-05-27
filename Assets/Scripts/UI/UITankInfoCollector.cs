using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform m_TankInfoPanel;
    [SerializeField] private UITankInfo m_TankInfoPrefab;

    private UITankInfo[] tanksInfo;
    private List<Player> playerWithoutLocal;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStart()
    {
        Player[] players = FindObjectsOfType<Player>();

        playerWithoutLocal = new List<Player>(players.Length - 1);

        for(int i =0; i < players.Length; i++) 
        {
            if (players[i] == Player.Local) continue;

            playerWithoutLocal.Add(players[i]);
        }

        tanksInfo = new UITankInfo[playerWithoutLocal.Count];

        for(int i = 0; i < playerWithoutLocal.Count; i++)
        {
            tanksInfo[i] = Instantiate(m_TankInfoPrefab);
            tanksInfo[i].SetTank(playerWithoutLocal[i].ActiveVehicle);
            tanksInfo[i].transform.SetParent(m_TankInfoPanel);

        }
    }

    private void OnMatchEnd()
    {
       for(int i = 0; i < m_TankInfoPanel.transform.childCount; i++)
       {
           Destroy(m_TankInfoPanel.transform.GetChild(i).gameObject);
       }

        tanksInfo = null;
    }

    private void Update()
    {
        if (tanksInfo == null) return;

        for(int i = 0; i < tanksInfo.Length; i++)
        {
            if (tanksInfo[i] == null) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(tanksInfo[i].Tank.transform.position + tanksInfo[i].WorldOffset);

            if(screenPos.z > 0)
            {
                tanksInfo[i].transform.position = screenPos;
            }
        }
    }
}
