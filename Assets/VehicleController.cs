using Cinemachine;
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

    [HideInInspector] public float currentForwardForce = 0f;

    public CinemachineFreeLook cinemachineCamera;

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
                if (wheel.currentSpringLength < wheel.springRestLength)
                    applyForces(wheel.transform.position, wheel, ackermannAngleLeft);
            }
            else if (wheel.frontRight)
            {
                wheel.steerAngle = ackermannAngleRight;
                // _rb.AddForceAtPosition(transform.InverseTransformPoint(new Vector3(calculateLateralFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position)), ackermannAngleRight), 0, calculateLongitudinalFriction(transform.InverseTransformPoint(_rb.GetPointVelocity(wheel.transform.position)), ackermannAngleRight))), wheel.transform.position);
                if (wheel.currentSpringLength < wheel.springRestLength)
                    applyForces(wheel.transform.position, wheel, ackermannAngleRight);
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

            if(wheel.currentSpringLength < wheel.springRestLength)
                applyForces(wheel.transform.position, wheel);
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

    public void OnCamera(InputValue input)
    {
        if (!cinemachineCamera) return;
        cinemachineCamera.m_XAxis.m_InputAxisValue = input.Get<Vector2>().x;
        cinemachineCamera.m_YAxis.m_InputAxisValue = input.Get<Vector2>().y;
    }

    public Vector3 calculateLongitudinalFriction(Vector3 currentLongitudinalVelocity, float tireAngle = 0f)
    {
        // float longitudeFriction = 0f;

        Vector3 rollingResistance = longitudinalFrictionCoefficient * (currentLongitudinalVelocity * Mathf.Cos(tireAngle * Mathf.Deg2Rad) + currentLongitudinalVelocity * Mathf.Sin(tireAngle * Mathf.Deg2Rad)) * _rb.mass;

        rollingResistance = Mathf.Clamp(rollingResistance.magnitude, -maxLongitudinalFriction, maxLongitudinalFriction) * rollingResistance.normalized;

        return rollingResistance;
    }

    public Vector3 calculateLateralFriction(Vector3 currentLateralVelocity, float tireAngle = 0f)
    {
        // float lateralFriction = 0f;

        Vector3 rollingResistance = lateralFrictionCoefficient * (currentLateralVelocity * Mathf.Sin(tireAngle * Mathf.Deg2Rad) + currentLateralVelocity * Mathf.Cos(tireAngle * Mathf.Deg2Rad)) * _rb.mass;

        rollingResistance = Mathf.Clamp(rollingResistance.magnitude, -maxLateralFriction, maxLateralFriction) * rollingResistance.normalized;

        return rollingResistance;
    }

    public Vector3 calculateLocalVelocity(Vector3 worldVelocity)
    {
        Vector3 forwardVelocity = Vector3.Project(worldVelocity, transform.forward);// worldVelocity.z * Mathf.Cos((transform.eulerAngles.y) * Mathf.Deg2Rad) - worldVelocity.x * Mathf.Sin((transform.eulerAngles.y) * Mathf.Deg2Rad); // Mathf.Cos(transform.eulerAngles.y);
        Vector3 rightVelocity = Vector3.Project(worldVelocity, transform.right);//  worldVelocity.x * Mathf.Cos((transform.eulerAngles.y) * Mathf.Deg2Rad) + worldVelocity.z * Mathf.Sin((transform.eulerAngles.y) * Mathf.Deg2Rad);
        Vector3 localVelocity = forwardVelocity + rightVelocity;//new Vector3(rightVelocity, 0, forwardVelocity);
        print(localVelocity == worldVelocity);
        return localVelocity;
    }

    public void applyForces(Vector3 forcePosition, WheelScript wheel, float angle = 0f)
    {
        Vector3 forwardVel = Vector3.Project(_rb.GetPointVelocity(forcePosition), wheel.transform.forward);
        Vector3 rightVel = Vector3.Project(_rb.GetPointVelocity(forcePosition), wheel.transform.right);

        Vector3 longitudinalForces = calculateLongitudinalFriction(forwardVel);//calculateLongitudinalFriction(_rb.GetPointVelocity(forcePosition), angle + transform.eulerAngles.y);//  - (throttleInput - brakeInput) * maxEngineForce;
        Vector3 lateralForces = calculateLateralFriction(rightVel);//calculateLateralFriction(_rb.GetPointVelocity(forcePosition), angle + transform.eulerAngles.y);
        // Vector3 forces = transform.InverseTransformVector(new Vector3(lateralForces, 0, longitudinalForces));

        // Debug.Log(forces);

        Vector3 forces = -longitudinalForces - lateralForces;// Vector3.zero;
        _rb.AddForceAtPosition((throttleInput - brakeInput) * maxEngineForce * wheel.transform.forward, forcePosition);
        _rb.AddForceAtPosition(forces, forcePosition);

        // print(calculateLateralFriction(calculateLocalVelocity(_rb.GetPointVelocity(forcePosition), transform), angle));
        //print(calculateLongitudinalFriction(transform.rotation * _rb.velocity, angle));
        //print(calculateLocalVelocity(_rb.velocity));
        //print(_rb.GetPointVelocity(forcePosition));

    }
}
