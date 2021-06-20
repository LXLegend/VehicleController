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

    public float steerInput = 0f;

    public float throttleInput = 0f;

    public float brakeInput = 0f;

    private float ackermannAngleLeft;
    private float ackermannAngleRight;

    public float maxEngineForce = 1000f;

    public float longitudinalFrictionCoefficient = .15f;

    public float lateralFrictionCoefficient = 0.95f;

    public float maxLateralFriction = 150000f;

    public float maxLongitudinalFriction = 15000f;

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
                // _rb.AddForceAtPosition(transform.InverseTransformPoint(new Vector3(calculateLateralFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position)), ackermannAngleLeft), 0, calculateLongitudinalFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position)), ackermannAngleLeft))), wheel.transform.position);
                applyForces(wheel.transform.position, ackermannAngleLeft);
            }
            else if (wheel.frontRight)
            {
                wheel.steerAngle = ackermannAngleRight;
                // _rb.AddForceAtPosition(transform.InverseTransformPoint(new Vector3(calculateLateralFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position)), ackermannAngleRight), 0, calculateLongitudinalFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position)), ackermannAngleRight))), wheel.transform.position);
                applyForces(wheel.transform.position, ackermannAngleRight);
            }

            // Debug.Log(wheel.calculateSpringForce());
            // Debug.Log(Quaternion.FromToRotation(Vector3.up, transform.up).eulerAngles);

            // scuffed friction and vehicle force application


        }

        foreach (WheelScript wheel in backWheels)
        {
            // calculate the force of suspension the wheel places on the vehicle body
            if (wheel.springRestLength > wheel.currentSpringLength)
                _rb.AddForceAtPosition(transform.up * wheel.calculateSpringForce(Time.fixedDeltaTime), wheel.transform.position);

            // _rb.AddForceAtPosition(transform.InverseTransformPoint(new Vector3(calculateLateralFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position))), 0, calculateLongitudinalFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position))) + (throttleInput - brakeInput) * maxEngineForce)), wheel.transform.position);

            // _rb.AddForceAtPosition(transform.forward * (throttleInput - brakeInput) * maxEngineForce, wheel.transform.position);

            applyForces(wheel.transform.position);
        }

        // Debug.Log((transform.InverseTransformPoint(new Vector3(calculateLateralFriction(transform.InverseTransformPoint(_rb.velocity)), 0, calculateLongitudinalFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(_rb.transform.position))) + (throttleInput - brakeInput) * maxEngineForce))));
    }

    private float calculateAckermanAngle(float sign)
    {
        return Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius + sign * (frontWheelDistance / 2)));
    }

    public void OnSteer(InputValue input)
    {
        steerInput = input.Get<float>();
    }

    public void OnThrottle(InputValue input)
    {
        throttleInput = input.Get<float>();
    }

    public void OnBrake(InputValue input)
    {
        brakeInput = input.Get<float>();
    }

    public float calculateLongitudinalFriction(Vector3 currentVelocity, float tireAngle = 0f)
    {
        // float longitudeFriction = 0f;

        float rollingResistance = longitudinalFrictionCoefficient * (currentVelocity.z * Mathf.Cos(tireAngle) + currentVelocity.x * Mathf.Sin(tireAngle));

        rollingResistance = Mathf.Clamp(rollingResistance, -maxLongitudinalFriction, maxLongitudinalFriction) * _rb.mass;

        return rollingResistance;
    }

    public float calculateLateralFriction(Vector3 currentVelocity, float tireAngle = 0f)
    {
        // float lateralFriction = 0f;

        float rollingResistance = lateralFrictionCoefficient * (currentVelocity.z * Mathf.Sin(tireAngle) + currentVelocity.x * Mathf.Cos(tireAngle));

        rollingResistance = Mathf.Clamp(rollingResistance, -maxLateralFriction, maxLateralFriction) * _rb.mass;

        return rollingResistance;
    }

    public Vector3 calculateLocalVelocity(float worldVelocity, Transform transform)
    {
        // float forwardVelocity = Mathf.Cos(transform.eulerAngles.y) Mathf.Sin(transform.eulerAngles.y) Mathf.Cos(transform.eulerAngles.y);
        Vector3 localVelocity = Vector3.zero;
        return localVelocity;
    }

    public void applyForces(Vector3 forcePosition, float angle = 0f)
    {
        float longitudinalForces = calculateLongitudinalFriction(transform.InverseTransformVector(_rb.velocity), angle)  + (throttleInput - brakeInput) * maxEngineForce;
        float lateralForces = calculateLateralFriction(transform.InverseTransformVector(_rb.velocity), angle);
        Vector3 forces = transform.InverseTransformVector(new Vector3(lateralForces, 0, longitudinalForces));

        Debug.Log(forces);

        _rb.AddForceAtPosition(forces, forcePosition);
    }
}
