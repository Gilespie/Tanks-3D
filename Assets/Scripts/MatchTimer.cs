using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchTimer : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private float m_MatchTimer;

    [SyncVar] private float timeLeft;
    public float TimeLeft => timeLeft;

    private bool timerEnd = false;

    bool IMatchCondition.IsTriggered => timerEnd;

    public void OnServerMatchEnd(MatchController controller)
    {
        enabled = false;
    }

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();
    }

    private void Start()
    {
        if(isServer == true)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if(isServer == true)
        {
            timeLeft -= Time.deltaTime;

            if(timeLeft <= 0)
            {
                timeLeft = 0;
                timerEnd = true;
            }
        }
    }

    private void Reset()
    {
        enabled = true;
        timeLeft = m_MatchTimer;
        timerEnd = false;
    }
}
