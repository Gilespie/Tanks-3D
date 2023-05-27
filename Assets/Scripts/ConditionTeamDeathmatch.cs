using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionTeamDeathmatch : MonoBehaviour, IMatchCondition
{
    private int red;
    private int green;

    private int winTeamId = -1;
    public int WinTeamId => winTeamId;

    private bool triggered;
    public bool IsTriggered => triggered;

    public void OnServerMatchStart(MatchController controller)
    {
        Reset();

        foreach (var v in FindObjectsOfType<Player>())
        {
            if (v.ActiveVehicle != null)
            {
                v.ActiveVehicle.OnEventDeath.AddListener(OnEventDeathHandler);

                if (v.TeamId == TeamSide.TeamRed)
                    red++;
                else if (v.TeamId == TeamSide.TeamGreen)
                    green++;
            }
        }
    }

    public void OnServerMatchEnd(MatchController controller)
    {
       
    }

    private void OnEventDeathHandler(Destructible e)
    {
        var ownerPlayer = e.Owner?.GetComponent<Player>();

        if (ownerPlayer == null) return;

        switch(ownerPlayer.TeamId)
        {
            case TeamSide.TeamRed:
                red--;
                break;

            case TeamSide.TeamGreen:
                green--;
                break;
        }

        if(red == 0)
        {
            winTeamId = 1;
            triggered = true;
        }
        else if(green == 0)
        {
            winTeamId = 0;
            triggered = true;
        }
    }

    private void Reset()
    {
        red = 0;
        green = 0;
        triggered = false;
    }
}
