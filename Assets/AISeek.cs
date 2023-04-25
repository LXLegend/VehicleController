using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISeek : MonoBehaviour
{
    public Transform target;

    private VehicleController vehicleControl;

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        vehicleControl = GetComponent<VehicleController>();

        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // vehicleControl.steerInput = GetSteeringAmount();

        // vehicleControl.throttleInput = 1f;
    }

    private void FixedUpdate()
    {
        // vehicleControl.steerInput = GetSteeringAmount();

    }

    public float GetSteeringAmount(Transform target)
    {
        Vector3 velocityVecOnPlane = Vector3.ProjectOnPlane(_rb.velocity, Vector3.up);

        Vector3 targetVecOnPlane = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up);

        float angle = Vector3.Dot(transform.right.normalized, targetVecOnPlane.normalized) * Vector3.Angle(velocityVecOnPlane, targetVecOnPlane);

        float maxSteerAngle = Mathf.Rad2Deg * Mathf.Atan(vehicleControl.wheelBaseLength / (vehicleControl.turnRadius));

        float steeringAmount = Mathf.Clamp(angle / maxSteerAngle, -1, 1);

        return steeringAmount;
    }
}
