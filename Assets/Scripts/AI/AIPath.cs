using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    public static AIPath Instance;

    [SerializeField] private Transform m_BaseRedPoint;
    [SerializeField] private Transform m_BaseGreenPoint;

    [SerializeField] private Transform[] m_FireRedPoints;
    [SerializeField] private Transform[] m_FireGreenPoints;

    [SerializeField] private Transform[] m_PatrolPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetBasePoint(int teamId)
    {
        if(teamId == TeamSide.TeamRed)
        {
            return m_BaseGreenPoint.position;
        }

        if(teamId == TeamSide.TeamGreen)
        {
            return m_BaseRedPoint.position;
        }

        return Vector3.zero;
    }

    public Vector3 GetRandomFirePoint(int teamId)
    {
        if(teamId == TeamSide.TeamRed)
        {
            return m_FireRedPoints[Random.Range(0, m_FireRedPoints.Length)].position;
        }
        if( teamId == TeamSide.TeamGreen)
        {
            return m_FireGreenPoints[Random.Range(0, m_FireGreenPoints.Length)].position;
        }

        return Vector3.zero;
    }

    public Vector3 GetRandomPatrolPoint()
    {
        return m_PatrolPoints[Random.Range(0, m_PatrolPoints.Length)].position;
    }
}