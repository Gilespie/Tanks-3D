using UnityEngine;
using UnityEngine.Events;
using Mirror;

public interface IMatchCondition
{
    bool IsTriggered { get; }

    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}

public class MatchController : NetworkBehaviour
{
    public UnityAction MatchStart;
    public UnityAction MatchEnd;

    public UnityAction SvMatchStart;
    public UnityAction SvMatchEnd;

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
                        Debug.Log("SvEndMatch()");
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

        foreach(var c in matchConditions)
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
