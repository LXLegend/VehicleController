using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private Rigidbody _rb;

    public WheelScript[] frontWheels;

    public float frontWheelDiameter = 1f;

    public float frontWheelWidth = 1f;

    public WheelScript[] backWheels;

    public float backWheelDiameter = 1f;

    public float backWheelWidth = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        foreach (WheelScript wheel in frontWheels)
        {
            // calculate the force of suspension the wheel places on the vehicle body
            if (wheel.springRestLength > wheel.currentSpringLength)
                _rb.AddForceAtPosition(transform.up * wheel.calculateSpringForce(Time.fixedDeltaTime), wheel.transform.position);
            // Debug.Log(wheel.calculateSpringForce());
            // Debug.Log(Quaternion.FromToRotation(Vector3.up, transform.up).eulerAngles);
        }

        foreach (WheelScript wheel in backWheels)
        {
            // calculate the force of suspension the wheel places on the vehicle body
            if (wheel.springRestLength > wheel.currentSpringLength)
                _rb.AddForceAtPosition(transform.up * wheel.calculateSpringForce(Time.fixedDeltaTime), wheel.transform.position);
        }
    }

}
