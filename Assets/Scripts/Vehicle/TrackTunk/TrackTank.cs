using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class TrackWheelRow
{
    [SerializeField] private WheelCollider[] colliders;
    [SerializeField] private Transform[] meshes;

    public float minRmp;

    //Public
    public void SetTorque(float motorTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].motorTorque = motorTorque;
        }
    }

    public void Break(float breakTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].brakeTorque = breakTorque;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].brakeTorque = 0;
            colliders[i].motorTorque = 0;
        }
    }

    public void SetSidewayStiffness(float stiffness)
    {
        WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

        for (int i = 0; i < colliders.Length; i++)
        {
            wheelFrictionCurve = colliders[i].sidewaysFriction;
            wheelFrictionCurve.stiffness = stiffness;

            colliders[i].sidewaysFriction = wheelFrictionCurve;
        }
    }

    public void UpdateMeshTransform()
    {
        //Find min RMP
        List<float> allRmp = new List<float>();

        for(int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].isGrounded == true)
            {
                allRmp.Add(colliders[i].rpm);
            }
        }

        if(allRmp.Count > 0)
        {
            minRmp = Mathf.Abs(allRmp[0]);

            for(int i = 0; i <allRmp.Count; i++)
            {
                if(Mathf.Abs(allRmp[i]) < minRmp)
                {
                    minRmp = Mathf.Abs(allRmp[i]);
                }
            }

            minRmp *= Mathf.Sign(allRmp[0]);
        }

        float angle = minRmp * 360.0f / 60 * Time.fixedDeltaTime;

        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            colliders[i].GetWorldPose(out position, out rotation);

            meshes[i].position = position;
            meshes[i].Rotate(angle, 0, 0);
        }
    }

    public void UpdateMeshRotationByRmp(float rmp)
    {
        float angle = rmp * 360.0f / 60.0f * Time.fixedDeltaTime;

        for(int i = 0;i < meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            colliders[i].GetWorldPose(out position, out rotation);
            
            meshes[i].position = position;
            meshes[i].Rotate(angle, 0, 0);

        }
    }

    //Private
    private void UpdateWheelTransform(WheelCollider wheelCollider, ref Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;

        wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class TrackTank : Vehicle
{
    public override float LinearVelocity => rigidBody.velocity.magnitude;

    [SerializeField] private Transform centerOfMass;

    [Header("Tracks")]
    [SerializeField] private TrackWheelRow leftWheelRow;
    [SerializeField] private TrackWheelRow rightWheelRow;
    [SerializeField] private GameObject m_DestroyPrefab;
    [SerializeField] private GameObject m_VisualModel;

    [Header("Movement")]
    [SerializeField] private ParameterCurve forwardTorqueCurve;
    [SerializeField] private float maxForwardTorque;
    [SerializeField] private float maxBackwardMotorTorque;
    [SerializeField] private ParameterCurve backwardTorqueCurve;
    [SerializeField] private float breakTorque;
    [SerializeField] private float rollingResistance;

    [Header("Rotation")]
    [SerializeField] private float rotateTorqueInPlace;
    [SerializeField] private float rotateBreakInPlace;
    [Space(2)]
    [SerializeField] private float rotateTorqueInMotion;
    [SerializeField] private float rotateBreakInMotion;

    [Header("Friction")]
    [SerializeField] private float minSidewayStiffnessInPlace;
    [SerializeField] private float minSidewayStiffnessInMotion;

    private Rigidbody rigidBody;
    [SerializeField] private float currentMotorTorque;

    public float LeftWheelRmp => leftWheelRow.minRmp;
    public float RightWheelRmp => rightWheelRow.minRmp;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass.localPosition;
    }

    private void FixedUpdate()
    {
       if(hasAuthority == true)
        {
            UpdateMotorTorque();
            CmdUpdateWheelRmp(LeftWheelRmp, RightWheelRmp);
        }
    }

    protected override void OnDestructibleDestroy()
    {
        base.OnDestructibleDestroy();

        GameObject ruinedPrefab = Instantiate(m_DestroyPrefab.gameObject, m_VisualModel.transform.position, m_VisualModel.transform.rotation);
    }

    [Command]
    private void CmdUpdateWheelRmp(float leftRmp, float rightRmp)
    {
        SvUpdateWheelRmp(leftRmp, rightRmp);
    }

    [Server]
    private void SvUpdateWheelRmp(float leftRmp, float rightRmp)
    {
        RcpUpdateWheelRmp(leftRmp, rightRmp);
    }

    [ClientRpc(includeOwner = false)]
    private void RcpUpdateWheelRmp(float leftRmp, float rightRmp)
    {
        leftWheelRow.minRmp = leftRmp;
        rightWheelRow.minRmp = rightRmp;

        leftWheelRow.UpdateMeshRotationByRmp(leftRmp);
        rightWheelRow.UpdateMeshRotationByRmp(rightRmp);
    }

    private void UpdateMotorTorque()
    {
        float targetMotorTorque = targetInputController.z > 0
           ? maxForwardTorque * Mathf.RoundToInt(targetInputController.z)
           : maxBackwardMotorTorque * Mathf.RoundToInt(targetInputController.z);

        float breakTorque = this.breakTorque * targetInputController.y;
        float steering = targetInputController.x;

        //Update target motor torque
        if (targetMotorTorque > 0)
        {
            currentMotorTorque = forwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque < 0)
        {
            currentMotorTorque = backwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque == 0)
        {
            currentMotorTorque = forwardTorqueCurve.Reset();
            currentMotorTorque = backwardTorqueCurve.Reset();
        }

        //break
        leftWheelRow.Break(breakTorque);
        rightWheelRow.Break(breakTorque);

        //Rolling
        if (targetMotorTorque == 0 && steering == 0)
        {
            leftWheelRow.Break(rollingResistance);
            rightWheelRow.Break(rollingResistance);
        }
        else
        {
            leftWheelRow.Reset();
            rightWheelRow.Reset();
        }

        // Rotate in place
        if (targetMotorTorque == 0 && steering != 0)
        {
            if (Mathf.Abs(leftWheelRow.minRmp) < 1 || Mathf.Abs(rightWheelRow.minRmp) < 1)
            {
                leftWheelRow.SetTorque(rotateTorqueInPlace);
                rightWheelRow.SetTorque(rotateTorqueInPlace);
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInPlace);
                    rightWheelRow.SetTorque(rotateTorqueInPlace);
                }

                if (steering > 0)
                {
                    leftWheelRow.SetTorque(rotateTorqueInPlace);
                    rightWheelRow.Break(rotateBreakInPlace);
                }
            }

            leftWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
            rightWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
        }

        //Move
        if (targetMotorTorque != 0)
        {
            if (steering == 0)
            {
                if (LinearVelocity < m_MaxLinearVelocity)
                {
                    leftWheelRow.SetTorque(currentMotorTorque);
                    rightWheelRow.SetTorque(currentMotorTorque);
                }
            }

            //delete this module if dont work move
            if (steering != 0 && (Mathf.Abs(leftWheelRow.minRmp) < 1 || Mathf.Abs(rightWheelRow.minRmp) < 1))
            {
                leftWheelRow.SetTorque(rotateTorqueInMotion);
                rightWheelRow.SetTorque(rotateTorqueInMotion);
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInMotion);
                    rightWheelRow.SetTorque(rotateTorqueInMotion);
                }

                if (steering > 0)
                {
                    leftWheelRow.SetTorque(rotateTorqueInMotion);
                    rightWheelRow.Break(rotateBreakInMotion);
                }
            }

            leftWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
            rightWheelRow.SetSidewayStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
        }

        leftWheelRow.UpdateMeshTransform();
        rightWheelRow.UpdateMeshTransform();
    }
}
