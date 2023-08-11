using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System.Collections;

public interface IMatchCondition
{
    bool IsTriggered { get; }

    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}

public class MatchController : NetworkBehaviour
{
    public static int TeamIDCounter;

    public static int GetTeamID()
    {
        return TeamIDCounter++ % 2;
    }

    public static void ResetTeamCounter()
    {
        TeamIDCounter = 1;
    }

    public event UnityAction MatchStart;
    public event UnityAction MatchEnd;

    public event UnityAction SvMatchStart;
    public event UnityAction SvMatchEnd;

    [SerializeField] private MatchMemberSpawner m_Spawner;
    [SerializeField] private float m_DelayAfterSpawnBeforeStartMatch = 0.5f;

    [SyncVar] private bool matchActive;
    public bool MatchActive => matchActive;

    public int WinTeamId = -1;

    private IMatchCondition[] matchConditions;

    private void Awake()
    {
        matchConditions = GetComponentsInChildren<IMatchCondition>();
    }

    private void Update()
    {
        if (isServer == true)
        {
            if(matchActive == true)
            {
                foreach(var c in matchConditions)
                {
                    if(c.IsTriggered == true)
                    {
                        SvEndMatch();
                        break;
                    }
                }
            }
        }
    }

    [Server]
    public void SvRestartMatch()
    {
        if (matchActive == true) return;

        matchActive = true;

        m_Spawner.SvRespawnVehiclesAllMembers();

        StartCoroutine(StartEventMatchWithDelay(m_DelayAfterSpawnBeforeStartMatch));
    }

    private IEnumerator StartEventMatchWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var c in matchConditions)
        {
            c.OnServerMatchStart(this);
        }

        SvMatchStart?.Invoke();

        RpcMatchStart();
    }

    [Server]
    public void SvEndMatch()
    {
        foreach (var c in matchConditions)
        {
            c.OnServerMatchEnd(this);

            if(c is ConditionTeamDeathmatch)
            {
                WinTeamId = (c as ConditionTeamDeathmatch).WinTeamId;
            }

            if(c is ConditionCaptureBase)
            {
                if((c as ConditionCaptureBase).RedBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.TeamGreen;
                }

                if((c as ConditionCaptureBase).GreenBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.TeamRed;
                }
            }
        }

        matchActive = false;

        SvMatchEnd?.Invoke();

        RpcMatchEnd(WinTeamId);
    }

    [ClientRpc]
    private  void RpcMatchStart()
    {
        MatchStart?.Invoke();
    }

    [ClientRpc]
    private void RpcMatchEnd(int winTeamId)
    {
        WinTeamId = winTeamId;
        MatchEnd?.Invoke();
    }
}
