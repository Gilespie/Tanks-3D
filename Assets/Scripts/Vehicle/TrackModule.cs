using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(TrackTank))]
public class TrackModule : NetworkBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject m_LeftTrackMesh;
    [SerializeField] private GameObject m_LeftTrackMeshRuined;
    [SerializeField] private GameObject m_RightTrackMesh;
    [SerializeField] private GameObject m_RightTrackMeshRuined;

    [Space(5)]
    [SerializeField] private VehicleModule m_LeftTrack;
    [SerializeField] private VehicleModule m_RightTrack;

    private TrackTank tank;

    private void Start()
    {
        tank = GetComponent<TrackTank>();

        m_LeftTrack.Destroyed += OnLeftTrackDestroyed;
        m_RightTrack.Destroyed += OnRightTrackDestroyed;

        m_LeftTrack.Recovered += OnLeftTrackRecovered;
        m_RightTrack.Recovered += OnRightTrackRecovered;
    }

    private void OnDestroy()
    {
        m_LeftTrack.Destroyed -= OnLeftTrackDestroyed;
        m_RightTrack.Destroyed -= OnRightTrackDestroyed;

        m_LeftTrack.Recovered -= OnLeftTrackRecovered;
        m_RightTrack.Recovered -= OnRightTrackRecovered;
    }

    private void OnLeftTrackDestroyed(Destructible arg0)
    {
        ChangeActiveObjects(m_LeftTrackMesh, m_LeftTrackMeshRuined);
        TakeAwayMobility();
    }
    private void OnLeftTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(m_LeftTrackMesh, m_LeftTrackMeshRuined);

        if(m_RightTrack.HitPoint > 0)
        {
            RegainMobility();
        }
    }

    private void OnRightTrackDestroyed(Destructible arg0)
    {
        ChangeActiveObjects(m_RightTrackMesh, m_RightTrackMeshRuined);
        TakeAwayMobility();
    }

    private void OnRightTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(m_RightTrackMesh, m_RightTrackMeshRuined);

        if (m_LeftTrack.HitPoint > 0)
        {
            RegainMobility();
        }
    }

    private void ChangeActiveObjects(GameObject a,  GameObject b)
    {
        a.SetActive(b.activeSelf);
        b.SetActive(!b.activeSelf);
    }

    private void TakeAwayMobility()
    {
        tank.enabled = false;
    }

    private void RegainMobility()
    {
        tank.enabled = true;
    }
}
