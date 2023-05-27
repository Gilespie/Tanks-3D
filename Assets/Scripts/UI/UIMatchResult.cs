using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMatchResult : MonoBehaviour
{
    [SerializeField] private GameObject m_PanelWin;
    [SerializeField] private TextMeshProUGUI m_TextWin;

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
        m_PanelWin.SetActive(false);
    }

    private void OnMatchEnd()
    {
        m_PanelWin.SetActive(true);

        int winTeamId = NetworkSessionManager.Match.WinTeamId;

        if(winTeamId == -1)
        {
            SetText("Ничья", Color.cyan);
        }
                
        if(winTeamId == Player.Local.TeamId) 
        {
            SetText("Победа", Color.green);
        }
        else
        {
            SetText("Поражение", Color.red);
        }
    }

    private void SetText(string text, Color color)
    {
        m_TextWin.color = color;
        m_TextWin.SetText(text);
    }
}
