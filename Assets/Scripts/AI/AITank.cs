using UnityEngine;
using Mirror;

public enum AIBehavoiurType
{
    Patrol,
    Support,
    InvaderBase
}

public class AITank : NetworkBehaviour
{
    [SerializeField] private AIBehavoiurType m_AIBehaviourType;

    [SerializeField, Range(0f, 1f)] private float m_PatrolChance;
    [SerializeField, Range(0f, 1f)] private float m_SupportChance;
    [SerializeField, Range(0f, 1f)] private float m_InvaderChance;

    [SerializeField] private Vehicle m_Vehicle;
    [SerializeField] private AIMovement m_Movement;
    [SerializeField] private AIShooter m_Shooter;

    private Vehicle fireTarget;
    private Vector3 movementTarget;

    private int StartTeamMember;
    private int currentTeamMember;

    public void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        m_Vehicle.Destroyed += OnVehicleDestroyed;
        m_Movement.enabled = false;
        m_Shooter.enabled = false;

        CalculateteamMember();
        SetStartBehaviour();
    }

    private void OnVehicleDestroyed(Destructible arg0)
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isServer == true)
        {
            UpdateBehaviour();
        }
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        m_Movement.enabled = false;
        m_Shooter.enabled = false;
        
    }


    private void OnMatchStart()
    {
        m_Movement.enabled = true;
        m_Shooter.enabled = true;
    }

    private void CalculateteamMember()
    {
        Vehicle[] v = FindObjectsOfType<Vehicle>();

        for(int i = 0; i < v.Length; i++)
        {
            if (v[i].TeamId == m_Vehicle.TeamId)
            {
                if (v[i] != m_Vehicle)
                {
                    StartTeamMember++;
                    v[i].Destroyed += OnTeamMemberDestroyed;
                }
            }
        }

        currentTeamMember = StartTeamMember;
    }

    private void SetStartBehaviour()
    {
        float chance = Random.Range(0f, m_PatrolChance + m_SupportChance + m_InvaderChance);

        if(chance >= 0f && chance <= m_PatrolChance)
        {
            StartBehaviour(AIBehavoiurType.Patrol);
            return;
        }

        if(chance >= m_PatrolChance && chance <= m_PatrolChance + m_SupportChance)
        {
            StartBehaviour(AIBehavoiurType.Support); 
            return;
        }

        if(chance >= m_PatrolChance + m_SupportChance && chance <= m_PatrolChance + m_SupportChance + m_InvaderChance)
        {
            StartBehaviour(AIBehavoiurType.InvaderBase);
            return;
        }
    }

    #region Behaviour

    private void StartBehaviour(AIBehavoiurType type)
    {
        m_AIBehaviourType = type;

        switch (type)
        {
            case AIBehavoiurType.Patrol:
                {
                    movementTarget = AIPath.Instance.GetRandomPatrolPoint();
                    break;
                }
            case AIBehavoiurType.Support:
                {
                    movementTarget = AIPath.Instance.GetRandomFirePoint(m_Vehicle.TeamId);
                    break;
                }
            case AIBehavoiurType.InvaderBase:
                {
                    movementTarget = AIPath.Instance.GetBasePoint(m_Vehicle.TeamId);
                    break;
                } 
        }

        m_Movement.ResetPath();
        
    }

    private void OnReachedDestination()
    {
        if(m_AIBehaviourType == AIBehavoiurType.Patrol)
        {
            movementTarget = AIPath.Instance.GetRandomPatrolPoint();
        }

        m_Movement.ResetPath();
    }

    private void OnTeamMemberDestroyed(Destructible dest)
    {
        currentTeamMember--;
        dest.Destroyed -= OnTeamMemberDestroyed;

        if((float)currentTeamMember / (float)StartTeamMember <= 0.4f)
        {
            StartBehaviour(AIBehavoiurType.Patrol);
        }
        if(currentTeamMember <= 2)
        {
            StartBehaviour(AIBehavoiurType.Patrol);
        }
    }

    private void UpdateBehaviour()
    {
        m_Shooter.FindTarget();

        if(m_Movement.ReachedDestination == true)
        {
            OnReachedDestination();
        }

        if(m_Movement.HasPath == false)
        {
            m_Movement.SetDestination(movementTarget);
        }
    }
    #endregion

}
