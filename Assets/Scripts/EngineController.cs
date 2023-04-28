using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;
using System.Runtime.Serialization;

public class EngineController : MonoBehaviour
{
    public float minRPM = 1000f;

    public float RPMIncrements = 500f;

    public float maxRPM = 7500f;

    public AnimationCurve[] torqueCurveAtThrottle;

    [Range(0f, 1f)]
    public float throttle = 0f;

    public int gear = 1;

    public float[] gearRatios;

    public float GForce = 0f;

    [Range(0f, 1f)]
    public float brake = 0f;

    public float brakeFrictionCoefficient = .95f;

    public float engineSpeed = 0f;

    public float transmissionInputSpeed = 0f;

    public float transmissionOutputSpeed = 0f;

    public bool transmissionEngaged = true;

    [Serializable]
    public struct MomentOfInertia
    {
        // radius in meters
        public float radius;
        // mass in kg
        public float mass;
        // moment of inertia
        public float momentOfInertia;
    }

    public MomentOfInertia flyWheel = new MomentOfInertia
    {
        radius = .10f,
        mass = 15f
    };

    public MomentOfInertia wheel = new MomentOfInertia
    {
        radius = .34f,
        mass = 15f
    };

    /*public TorqueCurve[] torqueMap;

    public float[] torqueValues;

    [Serializable]
    public struct TorqueCurve
    {
        public float[] torqueAtRPM;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        flyWheel.momentOfInertia = flyWheel.radius * flyWheel.radius * flyWheel.mass;
        wheel.momentOfInertia = wheel.radius * wheel.radius * wheel.mass;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void FixedUpdate()
    {
        // clamp engine speed
        Mathf.Clamp(engineSpeed, minRPM, maxRPM);

        // while shifting, remove throttle input for delay?
        // throttle = 0;

        // when shifting completes, calculate new engine speed from current wheel rotation with gear ratio- if engine speed falls below minRPM, do not allow shift, if engine speed is above max, do not engage transmission

        // calculate Torque from stored engine speed
        float engineTorque = torqueCurveAtThrottle[Mathf.RoundToInt(torqueCurveAtThrottle.Length * throttle)].Evaluate(engineSpeed);

        // calculate expected wheel rotation amount from engine speed
        float wheelRotation;

        // calculate expected displacement...?

        // calculate Wheel Torque from Engine Torque only if transmission engaged
        float wheelTorque = 0f;

        // calculate opposing Wheel Torque (Rolling Resistance + Brakes)
        float opposingWheelTorque;

        // calculate net Torque? 
        float netTorque = 50f;

        // calculate force(?) from wheel
        float wheelForce;

        // calculate Longitudinal Tire Force (slip)
        float wheelSlipRatio;

        // calculate new engine speed by calculating acceleration of engine
        // use net torque to find out angular velocity
        float totalMomentOfInertia = flyWheel.momentOfInertia;

        if (transmissionEngaged)
            totalMomentOfInertia += wheel.momentOfInertia * 4f;

        float angularAcceleration = netTorque / totalMomentOfInertia;

        engineSpeed = Time.fixedDeltaTime * angularAcceleration;

        // 1 RPM = 0.10472 rad/s
        // 1 rad/s = 9.549297 RPM
    }

    /*public float CalculateTorque(float RPM, float throttle)
    {
        Vector3 planeNormal = // (RPM - torqueValues[0]) / RPMIncrements;

    }*/

    /*public Vector2 GetClosestRPM(float RPM)
    {
        float closestValue = torqueValues[0];
        float secondClosestValue = torqueValues[1];
        foreach (float torqueValue in torqueValues)
        {
            if (torqueValue - RPM < closestValue - RPM)
            {
                closestValue = torqueValue;
                secondClosestValue = closestValue;
            }
        }

        return new Vector2(closestValue, secondClosestValue);
    }*/
}
