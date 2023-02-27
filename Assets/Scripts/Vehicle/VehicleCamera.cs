using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VehicleCamera : MonoBehaviour
{
    public static VehicleCamera Instance;

    [SerializeField] private Vehicle m_Vehicle;
    [SerializeField] private Vector3 m_Offset;

    [Header("Sensetive Limit")]
    [SerializeField] private float m_RotateSensitive;
    [SerializeField] private float m_ScrollSensitive;

    [Header("Rotation Limits")]
    [SerializeField] private float m_MaxVerticalAngle;
    [SerializeField] private float m_MinVerticalAngle;

    [Header("Distance")]
    [SerializeField] private float m_Distance;
    [SerializeField] private float m_MaxDistance;
    [SerializeField] private float m_MinDistance;
    [SerializeField] private float m_DistanceOffsetFromCollisionHit;
    [SerializeField] private float m_DistanceLerpRate;

    [Header("Zoom Optics")]
    [SerializeField] private GameObject zoomMaskEffect;
    [SerializeField] private float zoomedFov;
    [SerializeField] private float zoomedMaxVerticalAngle;

    private new Camera camera;

    private Vector2 RotationControl;

    private float defaultFov;
    private float deltaRotationX;
    private float deltaRotationY;
    private float currentDistance;
    private float defaultMaxVerticalAngle;
    private float lastDistance;

    private bool isZoom;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
        defaultFov = camera.fieldOfView;
        defaultMaxVerticalAngle = m_MaxVerticalAngle;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (m_Vehicle == null) return;

        UpdateControl();

        m_Distance = Mathf.Clamp(m_Distance, m_MinDistance, m_MaxDistance);
        isZoom = m_Distance <= m_MinDistance;

        // Calculate rotation and translation
        deltaRotationX += RotationControl.x * m_RotateSensitive;
        deltaRotationY += RotationControl.y * -m_RotateSensitive;

        deltaRotationY = ClampAngle(deltaRotationY, m_MinVerticalAngle, m_MaxVerticalAngle);        

        Quaternion finalrotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
        Vector3 finalposition = m_Vehicle.transform.position - (finalrotation * Vector3.forward * m_Distance);
        finalposition = AddLocalOffset(finalposition);

        // Calculate current distance
        float targetDistance = m_Distance;

        RaycastHit hit;

        Debug.DrawLine(m_Vehicle.transform.position + new Vector3(0, m_Offset.y, 0), finalposition, Color.cyan);

        if (Physics.Linecast(m_Vehicle.transform.position + new Vector3(0, m_Offset.y, 0), finalposition, out hit) == true)
        {
            float distanceToHit = Vector3.Distance(m_Vehicle.transform.position + new Vector3(0, m_Offset.y, 0), hit.point);

            if (hit.transform != m_Vehicle)
            {
                if (distanceToHit < m_Distance)
                {
                    targetDistance = distanceToHit - m_DistanceOffsetFromCollisionHit;
                }
            }
        }

        currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * m_DistanceLerpRate);
        currentDistance = Mathf.Clamp(currentDistance, m_MinDistance, m_Distance);
        // Correct camera position
        finalposition = m_Vehicle.transform.position - (finalrotation * Vector3.forward * currentDistance);

        // Apply transform
        transform.rotation = finalrotation;
        transform.position = finalposition;
        transform.position = AddLocalOffset(transform.position);

        //Zoom
        zoomMaskEffect.SetActive(isZoom);
        if(isZoom == true)
        {
            transform.position = m_Vehicle.ZoomOpticPosition.position;
            camera.fieldOfView = zoomedFov;
            m_MaxVerticalAngle = zoomedMaxVerticalAngle;
        }
        else
        {
            camera.fieldOfView = defaultFov;
            m_MaxVerticalAngle = defaultMaxVerticalAngle;
        }
    }

    private void UpdateControl()
    {
        RotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_Distance += -Input.mouseScrollDelta.y * m_ScrollSensitive;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            isZoom = !isZoom;
            
            if(isZoom == true)
            {
                lastDistance = m_Distance;
                m_Distance = m_MinDistance; 
            }
            else
            {
                m_Distance = lastDistance;
                currentDistance = lastDistance;
            }
        }
    }

    private Vector3 AddLocalOffset(Vector3 position)
    {
        Vector3 result = position;
        result += new Vector3(0, m_Offset.y, 0);
        result += transform.right * m_Offset.x;
        result += transform.forward * m_Offset.z;
        return result;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }

    public void SetTarget(Vehicle target)
    {
        m_Vehicle = target;
    }
}