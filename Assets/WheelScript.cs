using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "newWheel", menuName = "wheel")]
public class WheelScript : MonoBehaviour // ScriptableObject
{
    public LayerMask layerMask;

    [Header("Suspension")]

    // spring Length at rest
    public float springRestLength = 2f;

    // the maximum displacement of the spring
    public float maxSpringTravel = 0.3f;

    // the k value of the spring
    public float springStiffness = 30000f;

    // the dampening amount
    public float dampeningAmount = 4000f;

    // total length of the Raycast UNUSED
    [HideInInspector] public float raycastlength;

    // minimum spring length
    [HideInInspector] public float minSpringLength;

    // maximum spring length (mostly unused because we do not account for sprung and unsprung mass
    [HideInInspector] public float maxSpringLength;

    // the current spring length
    [HideInInspector] public float currentSpringLength;

    // the length of the spring in the previous physics update
    [HideInInspector] public float previousSpringLength;

    [HideInInspector] public bool isGrounded;

    // public Vector3 wheelCenter;

    public GameObject wheelObject;

    [header("Steering")]

    public float steerAngle;

    // there's probably a better way to do this but whatever
    public bool frontLeft;
    public bool frontRight;
    public bool backLeft;
    public bool backRight;

    [HideInInspector] public Vector3 contactPoint = Vector3.zero;

    [header("Slip")]

    public float maxTireLongitudinalLoad;

    public float maxTireLateralLoad;

    // velocity of the displacement
    private float displacementVelocity = 0f;

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, steerAngle, transform.localRotation.z);
    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, springRestLength /*maxSpringLength*/ + wheelObject.transform.localScale.y / 2, layerMask))
        {
            previousSpringLength = currentSpringLength;
            currentSpringLength = raycastHit.distance - wheelObject.transform.localScale.y / 2;
            // currentSpringLength = Mathf.Clamp(currentSpringLength, minSpringLength, maxSpringLength);

            isGrounded = true;

            contactPoint = raycastHit.point;
        }
        else
        {
            previousSpringLength = springRestLength;
            currentSpringLength = springRestLength;

            isGrounded = false;
        }

        // Debug.Log(raycastHit.collider.name);

        // Debug.Log(raycastHit.distance);

        // Debug.Log(currentDisplacement);

        wheelObject.transform.localPosition = new Vector3(0, - currentSpringLength, 0);
    }

    public float calculateDisplacementVelocity(float deltaTime)
    {
        return (currentSpringLength - previousSpringLength) / deltaTime;

        // return (currentDisplacement - prevDisplacement) / deltaTime;
    }

    public float calculateSpringForce(float deltaTime)
    {
        float springForce = - springStiffness * (currentSpringLength - springRestLength);
        float damperForce = dampeningAmount * calculateDisplacementVelocity(deltaTime); // Mathf.Min(springForce, dampeningAmount * calculateDisplacementVelocity(deltaTime));
        damperForce = Mathf.Clamp(damperForce, -springForce, springForce);
        float suspensionForce = (springForce - damperForce);

        return Mathf.Max(suspensionForce, 0);
    }
}
