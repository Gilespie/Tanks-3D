using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Vehicle))]
public class VehicleViewer : NetworkBehaviour
{
    private const float UPDATE_INTERVAL = 0.33f;
    private const float X_RAY_RADIUS = 50.0f;
    private const float BASE_EXIT_TIME_FROM_DISCOVERY = 10.0f;
    private const float CAMOUFLAGE_DISTANCE = 150.0f;

    [SerializeField] private float m_ViewDistance;
    [SerializeField] private Transform[] m_ViewPoints;
    [SerializeField] private Color m_Color;
    public List<VehicleDimensions> allVehicleDimensions = new List<VehicleDimensions>();
    public SyncList<NetworkIdentity> VisibleVehicles = new SyncList<NetworkIdentity>();
    public List<float> remainingTime = new List<float>();
    private Vehicle vehicle;
    private float remainingTimeLastUpdate;

    public override void OnStartServer()
    {
        base.OnStartServer();

        vehicle = GetComponent<Vehicle>();

        NetworkSessionManager.Match.SvMatchStart += OnSvMatchStart;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkSessionManager.Match.SvMatchStart -= OnSvMatchStart;
    }

    private void OnSvMatchStart()
    {
        m_Color = Random.ColorHSV();

        Vehicle[] allVeh = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVeh.Length; i++)
        {
            if (vehicle == allVeh[i]) continue;

            VehicleDimensions vd = allVeh[i].GetComponent<VehicleDimensions>();

            if (vd == null) continue;

            if (vehicle.TeamId != allVeh[i].TeamId)
            {
                allVehicleDimensions.Add(vd);
            }
            else
            {
                VisibleVehicles.Add(vd.Vehicle.netIdentity);
                remainingTime.Add(-1);
            }
        }
    }

    private void Update()
    {
        if (isServer == false) return;

        remainingTimeLastUpdate += Time.deltaTime;

        if (remainingTimeLastUpdate > UPDATE_INTERVAL)
        {
            for (int i = 0; i < allVehicleDimensions.Count; i++)
            {
                if (allVehicleDimensions[i].Vehicle == null) continue;

                bool IsVisible = false;

                for (int j = 0; j < m_ViewPoints.Length; j++)
                {
                    IsVisible = CheckVisibility(m_ViewPoints[j].position, allVehicleDimensions[i]); //TODO: cambiar esta mierda)

                    if (IsVisible == true) break;
                }

                if (IsVisible == true && VisibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == false)
                {
                    VisibleVehicles.Add(allVehicleDimensions[i].Vehicle.netIdentity);
                    remainingTime.Add(-1);
                }

                if (IsVisible == true && VisibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    remainingTime[VisibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] = -1;
                }

                if (IsVisible == false && VisibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    if (remainingTime[VisibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] == -1)
                        remainingTime[VisibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] = BASE_EXIT_TIME_FROM_DISCOVERY;
                }
            }

            for (int i = 0; i < remainingTime.Count; i++)
            {
                if (remainingTime[i] > 0)
                {
                    remainingTime[i] -= Time.deltaTime;

                    if (remainingTime[i] <= 0)
                    {
                        remainingTime[i] = 0;
                    }
                }

                if (remainingTime[i] == 0)
                {
                    remainingTime.RemoveAt(i);
                    VisibleVehicles.RemoveAt(i);
                }
            }

            remainingTimeLastUpdate = 0f;
        }
    }

    public bool IsVisible(NetworkIdentity identity)
    {
        return VisibleVehicles.Contains(identity);
    }

    public List<Vehicle> GetAllVehicle()
    {
        List<Vehicle> av = new List<Vehicle>(allVehicleDimensions.Count);

        for (int i = 0; i < allVehicleDimensions.Count; i++)
        {
            av.Add(allVehicleDimensions[i].Vehicle);
        }

        return av;
    }

    private bool CheckVisibility(Vector3 viewPoint, VehicleDimensions vehicleDimensions)
    {
        float distance = Vector3.Distance(transform.position, vehicleDimensions.transform.position);

        if (Vector3.Distance(viewPoint, vehicleDimensions.transform.position) <= X_RAY_RADIUS) return true;

        if (distance > m_ViewDistance) return false;

        float curViewDist = m_ViewDistance;

        if(distance >= CAMOUFLAGE_DISTANCE)
        {
            VehicleCamouflage vehicleCamouflage = vehicleDimensions.Vehicle.GetComponent<VehicleCamouflage>();

            if (vehicleCamouflage != null)
                curViewDist = m_ViewDistance - vehicleCamouflage.CurrentDistance;
        }

        if(distance > curViewDist) return false;

        return vehicleDimensions.IsVisibleFromPoint(transform.root, viewPoint, m_Color); //TODO: Fix it
    }
}