using UnityEngine;

public class VehicleInputController : MonoBehaviour
{
    [SerializeField] private Vehicle Vehicle;
    
    protected virtual void Update()
    {
        Vehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
    }
}