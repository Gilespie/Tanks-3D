using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private AIRaySensor m_ForwardSensors;
    [SerializeField] private AIRaySensor m_BackwardSensors;
    [SerializeField] private AIRaySensor m_LeftSensors;
    [SerializeField] private AIRaySensor m_RightSensors;
    [SerializeField] private float m_StopDistance = 1f;
    [SerializeField] private float m_PathUpdateRate;

    private float timerUpdatePath;
    private Vector3 nextPathPoint;
    private Vector3 target;
    private Vehicle vehicle;
    private NavMeshPath path;

    private bool hasPath;
    private bool reachedDestination;
    private int cornerIndex;

    public bool HasPath => hasPath;
    public bool ReachedDestination => reachedDestination;

    private void Awake()
    {
        path = new NavMeshPath();
        vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        if(m_PathUpdateRate > 0)
        {
            timerUpdatePath += Time.deltaTime;

            if(timerUpdatePath > m_PathUpdateRate)
            {
                CalculatePath(target);
                timerUpdatePath = 0;
            }
        }

        UpdateTarget();
        MoveToTarget();  
    }

    public void SetDestination(Vector3 target)
    {
        if (this.target == target && hasPath == true) return;
        
        this.target = target;

        CalculatePath(target);
    }

    public void ResetPath()
    {
        hasPath = false;
        reachedDestination = false;
    }

    private void UpdateTarget()
    {
        if(hasPath == false) return;

        nextPathPoint = path.corners[cornerIndex];

        if(Vector3.Distance(transform.position, nextPathPoint) < m_StopDistance)
        {
            if(path.corners.Length - 1 > cornerIndex)
            {
                cornerIndex++;
                nextPathPoint = path.corners[cornerIndex];
            }
            else
            {
                hasPath = false;
                reachedDestination = true;
            }
        }

        for(int i = 0; i < path.corners.Length - 1; i++) 
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.cyan);
        }
    }

    private void CalculatePath(Vector3 target)
    {
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);

        hasPath = path.corners.Length > 0;
        reachedDestination = false;

        cornerIndex = 1;
    }

    public Vector3 GetReferenceMovementDirectionZX()
    {
        var tankPos = vehicle.transform.GetPositionZX();
        var targetPos = nextPathPoint.GetPositionZX();
        
        return (targetPos - tankPos).normalized;
    }
    private Vector3 GetTankDirectionZX()
    {
        var tankDir = vehicle.transform.forward.GetPositionZX();
        tankDir.Normalize();
        return tankDir;
    }

    private void MoveToTarget()
    {
        if (nextPathPoint == null) return;

        if(reachedDestination == true)
        {
            vehicle.SetTargetControl(new Vector3(0, 1, 0));
            return;
        }

        float turnControl = 0f;
        float forwardThrust = 1f;

        var referenceDirection = GetReferenceMovementDirectionZX();
        var tankDir = GetTankDirectionZX();

        var forwardSensorState = m_ForwardSensors.Raycast();
        var leftSensorState = m_LeftSensors.Raycast();
        var rightSensorState = m_RightSensors.Raycast();

        if(forwardSensorState.Item1)
        {
            forwardThrust = 0f;

            if(leftSensorState.Item1 == false)
            {
                turnControl = -1f;
                forwardThrust = -0.2f;
            }
            else if(rightSensorState.Item1 == false)
            {
                turnControl = 1f;
                forwardThrust = -0.2f;
            }
            else
            {
                forwardThrust = -1f;
            }
        }
        else
        {
            turnControl = Mathf.Clamp(Vector3.SignedAngle(tankDir, referenceDirection, Vector3.up), -45.0f, 45.0f) / 45.0f;

            float minSideDistance = 1f;

            if (leftSensorState.Item1 && leftSensorState.Item2 < minSideDistance && turnControl < 0)
                turnControl = -turnControl;

            if(rightSensorState.Item1 && rightSensorState.Item2 < minSideDistance && turnControl > 0)
                turnControl = -turnControl;
        }

        vehicle.SetTargetControl(new Vector3(turnControl, 0, forwardThrust));
    }
}
