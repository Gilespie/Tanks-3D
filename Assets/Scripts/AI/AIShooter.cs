using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class AIShooter : MonoBehaviour
{
    [SerializeField] private VehicleViewer m_Viewer;
    [SerializeField] private Transform m_FirePosition;

    private Vehicle vehicle;
    private Vehicle target;
    private Transform lookTransform; 

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    void Update()
    {
        LookAtTarget();
        TryFire();
    }

    public void FindTarget()
    {
        List<Vehicle> v = m_Viewer.GetAllVisibleVehicle();

        float minDist = float.MaxValue;
        int index = -1;

        for(int i = 0; i < v.Count; i++)
        {
            if (v[i].HitPoint <= 0) return;
            if (v[i].TeamId == vehicle.TeamId) return;

            float dist = Vector3.Distance(transform.position, v[i].transform.position);

            if(dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        if(index !=  -1)
        {
            target = v[index];

            VehicleDimensions vehicleDimensions = target.GetComponent<VehicleDimensions>();

            if (vehicleDimensions == null) return;

            lookTransform = vehicleDimensions.GetPriorityFirePoint();
        }
        else
        {
            target = null;
            lookTransform = null;
        }
    }

    private void LookAtTarget()
    {
        if (lookTransform == null) return;

        vehicle.NetAimPoint = lookTransform.position;
    }

    private void TryFire()
    {
        if (target == null) return;

        RaycastHit hit;

        if(Physics.Raycast(m_FirePosition.position, m_FirePosition.forward, out hit, 1000))
        {
            if(hit.collider.transform.root == target.transform.root)
            {
                vehicle.Turret.SvFire();
            }
        }
    }
}
