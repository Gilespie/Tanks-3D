using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ConditionCaptureBase : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private TeamBase m_RedBase;
    [SerializeField] private TeamBase m_GreenBase;

    [SyncVar] private float redBaseCaptureLevel;
    public float RedBaseCaptureLevel => redBaseCaptureLevel;

    [SyncVar] private float greenBaseCaptureLevel;
    public float GreenBaseCaptureLevel => greenBaseCaptureLevel;

    private bool triggered;
    public bool IsTriggered => triggered;

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
        enabled = true;
    }

    private void Update()
    {
        if(isServer == true)
        {
            redBaseCaptureLevel = m_RedBase.CaptureLevel;
            greenBaseCaptureLevel = m_GreenBase.CaptureLevel;

            if(redBaseCaptureLevel == 100 || greenBaseCaptureLevel == 100)
            {
                triggered = true;
            }
        }
    }

    private void Reset()
    {
        m_RedBase.Reset();
        m_GreenBase.Reset();

        redBaseCaptureLevel = 0;
        greenBaseCaptureLevel = 0;

        triggered = false;
        enabled = true;
    }
}
