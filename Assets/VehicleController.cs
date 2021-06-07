using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    private Rigidbody _rb;

    public WheelScript[] frontWheels;
    public float frontWheelDiameter = 1f;
    public float frontWheelWidth = 1f;
    public float frontWheelDistance = 1.6f;
    public float frontWheelAnchorHeight = 0f;


    public WheelScript[] backWheels;
    public float backWheelDiameter = 1f;
    public float backWheelWidth = 1f;
    public float backWheelDistance = 1.6f;
    public float backWheelAnchorHeight = 0f;


    public float wheelBaseLength = 2f;

    public float turnRadius = 14;

    public float steerInput;

    private float ackermannAngleLeft;
    private float ackermannAngleRight;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (steerInput < 0) // turning Left
        {
            ackermannAngleLeft = calculateAckermanAngle(-1) * steerInput;
            ackermannAngleRight = calculateAckermanAngle(1) * steerInput;
        }
        else if (steerInput > 0) // turning Right
        {
            ackermannAngleLeft = calculateAckermanAngle(1) * steerInput;
            ackermannAngleRight = calculateAckermanAngle(-1) * steerInput;
        }
        else // straight
        {
            ackermannAngleLeft = 0f;
            ackermannAngleRight = 0f;
        }
    } 

    private void FixedUpdate()
    {
        foreach (WheelScript wheel in frontWheels)
        {
            // calculate the force of suspension the wheel places on the vehicle body
            if (wheel.springRestLength > wheel.currentSpringLength)
                _rb.AddForceAtPosition(transform.up * wheel.calculateSpringForce(Time.fixedDeltaTime), wheel.transform.position);

            if (wheel.frontLeft)
            {
                wheel.steerAngle = ackermannAngleLeft;
            }
            else if (wheel.frontRight)
            {
                wheel.steerAngle = ackermannAngleRight;
            }

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

    private float calculateAckermanAngle(float sign)
    {
        return Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius + sign * (frontWheelDistance / 2)));
    }

    public void OnSteer(InputValue input)
    {
        steerInput = input.Get<float>();
    }

}
