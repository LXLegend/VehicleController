using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    public VehicleController vehicleController;
    public Rigidbody _rb;

    public AISeek seekScript;

    public float threshold = 1f;

    public float raycastDist = 10f;

    private float carWidth = 1f;

    private float turnRadius = 14f;

    public float turnConfidence = 0.6f;

    public LayerMask obstacle;

    private float steeringInput = 0f;

    private float throttleInput = 0f;

    private float brakeInput = 0f;

    public float reverseTime = 1f;

    private float currentReverseTime = 0f;

    public Transform currentTarget;

    public Transform player;

    // public bool triedLeft, triedRight, turnAround;

    // public float sightRefresh = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        vehicleController = GetComponent<VehicleController>();

        _rb = GetComponent<Rigidbody>();

        if (vehicleController)
        {
            if (raycastDist < vehicleController.turnRadius * 2f)
                raycastDist = vehicleController.turnRadius * 2f;
            carWidth = Mathf.Max(vehicleController.backWheelDistance, vehicleController.frontWheelDistance);
            turnRadius = vehicleController.turnRadius;
        }
        seekScript = GetComponent<AISeek>();

        if (player)
            currentTarget = player;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        bool fRHit = false, fLHit = false, bRHit = false, bLHit = false, sLHit = false, sRHit = false;
        int turnRight = 0, turnLeft = 0;
        Vector3 fRHitNormal;

        RaycastHit forwardRightHit, forwardLeftHit, backRightHit, backLeftHit, sideRightHit, sideLeftHit;

        // (Vector3.Dot(transform.forward, _rb.velocity) >= 0);
        fRHit = (Physics.Raycast(transform.position + Vector3.right * carWidth / 2f, transform.forward, out forwardRightHit, raycastDist, obstacle, QueryTriggerInteraction.Ignore));

        fLHit = (Physics.Raycast(transform.position - Vector3.right * carWidth / 2f, transform.forward, out forwardLeftHit, raycastDist, obstacle, QueryTriggerInteraction.Ignore));

        bRHit = (Physics.Raycast(transform.position + Vector3.right * carWidth / 2f, -transform.forward, out backRightHit, raycastDist, obstacle, QueryTriggerInteraction.Ignore));

        bLHit = (Physics.Raycast(transform.position - Vector3.right * carWidth / 2f, -transform.forward, out backLeftHit, raycastDist, obstacle, QueryTriggerInteraction.Ignore)) ;

        sRHit = (Physics.Raycast(transform.position, transform.right, out sideRightHit, raycastDist, obstacle, QueryTriggerInteraction.Ignore));

        sLHit = (Physics.Raycast(transform.position, -transform.right, out sideLeftHit, raycastDist, obstacle, QueryTriggerInteraction.Ignore));

        if (seekScript)
            steeringInput = seekScript.GetSteeringAmount(currentTarget);

        if (fRHit || fLHit)
        {
            float fRHitDist = raycastDist;

            float fLHitDist = raycastDist;

            if (fRHit)
                fRHitDist = forwardRightHit.distance;

            if (fLHit)
                fLHitDist = forwardLeftHit.distance;

            float minDist = Mathf.Min(fRHitDist, fLHitDist);
            if (fRHit)
            {
                // apply the brakes
                brakeInput = 1f - turnConfidence * Mathf.Clamp(minDist / (turnRadius + threshold), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal), 0f, 1f);
                throttleInput = 1f * Mathf.Clamp(minDist / (turnRadius), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal), 0f, 1f);
                steeringInput = Mathf.Sign(- Vector3.Dot(transform.forward, _rb.velocity)) * Mathf.Sign(Vector3.Dot(transform.right, forwardRightHit.normal)) * (1f - 1f * Mathf.Clamp(forwardRightHit.distance / (turnRadius + threshold), 0f, 1f)) * Mathf.Abs(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal));
            }
            //if (fLHit && (sRHit || sLHit))
            //{
            //    steeringInput = Mathf.Sign(steeringInput);
            //}
            if (fLHit)
            {
                brakeInput = 1f - turnConfidence * Mathf.Clamp(minDist / (turnRadius + threshold), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardLeftHit.normal), 0f, 1f);
                throttleInput = 1f * Mathf.Clamp(minDist / (turnRadius), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal), 0f, 1f);
                steeringInput = Mathf.Sign(- Vector3.Dot(transform.forward, _rb.velocity)) * Mathf.Sign(Vector3.Dot(transform.right, forwardRightHit.normal)) * (1f - 1f * Mathf.Clamp(forwardRightHit.distance / (turnRadius + threshold), 0f, 1f)) * Mathf.Abs(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal));
            }
        }
        else
        {
            throttleInput = 1;
            brakeInput = 0f;
        }

        //if (CanSeeTarget(currentTarget.gameObject))
        //    throttleInput = 0.5f * Vector3.Dot(transform.forward, currentTarget.position - transform.position);

        // if there is an object to the side of the vehicle, steer away from that side
        if (sLHit)
        {
            steeringInput += 1f * Mathf.Clamp((sideLeftHit.distance - carWidth / 2) / threshold, 0f, 1f);//  * Mathf.Abs(Vector3.Dot(sideLeftHit.normal, transform.right));
        }
        if (sRHit)
        {
            steeringInput -= 1f * Mathf.Clamp((sideRightHit.distance - carWidth / 2) / threshold, 0f, 1f);//  * Mathf.Abs(Vector3.Dot(sideRightHit.normal, transform.right));
        }

        //// if there is an object in front of the vehicle
        //if (fRHit || fLHit)
        //{
        //    float fRHitDist = raycastDist;

        //    float fLHitDist = raycastDist;

        //    if (fRHit)
        //        fRHitDist = forwardRightHit.distance;

        //    if (fLHit)
        //        fLHitDist = forwardLeftHit.distance;

        //    float minDist = Mathf.Min(fRHitDist, fLHitDist);

        //    // and if the object is sufficiently close
        //    if (minDist < threshold + turnRadius)
        //    {




        //        //if (fRHit && (sRHit || sLHit))
        //        //{
        //        //    steeringInput = Mathf.Sign(steeringInput);
        //        //}
        //        if (fRHit)
        //        {
        //            // apply the brakes
        //            brakeInput = 1f - turnConfidence * Mathf.Clamp(minDist / (turnRadius + threshold), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal), 0f, 1f);
        //            throttleInput = 1f * Mathf.Clamp(minDist / (turnRadius), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal), 0f, 1f);
        //            steeringInput += Mathf.Sign(Vector3.Dot(transform.forward, _rb.velocity)) * Mathf.Sign(Vector3.Dot(transform.right, forwardRightHit.normal)) * (1f - 1f * Mathf.Clamp(forwardRightHit.distance / (turnRadius + threshold), 0f, 1f)) * Mathf.Abs(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal));
        //        }
        //        //if (fLHit && (sRHit || sLHit))
        //        //{
        //        //    steeringInput = Mathf.Sign(steeringInput);
        //        //}
        //        if (fLHit)
        //        {
        //            brakeInput = 1f - turnConfidence * Mathf.Clamp(minDist / (turnRadius + threshold), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardLeftHit.normal), 0f, 1f);
        //            throttleInput = 1f * Mathf.Clamp(minDist / (turnRadius), 0f, 1f) * Mathf.Clamp(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal), 0f, 1f);
        //            steeringInput += Mathf.Sign(Vector3.Dot(transform.forward, _rb.velocity)) * Mathf.Sign(Vector3.Dot(transform.right, forwardRightHit.normal)) * (1f - 1f * Mathf.Clamp(forwardRightHit.distance / (turnRadius + threshold), 0f, 1f)) * Mathf.Abs(Vector3.Dot(transform.forward.normalized, forwardRightHit.normal));
        //        }
        //    }
        //    else
        //    {
        //        throttleInput = 1f;
        //        brakeInput = 0f;
        //    }
        //}
        //else
        //{
        //    throttleInput = 1f;
        //    brakeInput = 0f;
        //}

        //if (sRHit)
        //    // if the object on the right is getting closer and the distance is less than the threshold
        //    if (Vector3.Dot(_rb.velocity, sideRightHit.normal) > 0 && sideRightHit.distance < threshold)
        //    {
        //        // turn away
        //    }

        throttleInput = Mathf.Clamp(throttleInput - 0.4f * steeringInput, 0f, 1f);

        vehicleController.brakeInput = brakeInput;

        vehicleController.throttleInput = throttleInput;

        vehicleController.steerInput = Mathf.Clamp(steeringInput, -1f, 1f);

    }

    public bool CanSeePlayer()
    {
        RaycastHit hit;
        if (player)
            if (Physics.Linecast(transform.position, player.position, out hit))
                return hit.transform.CompareTag("Player");
        return false;
    }

    public bool CanSeeTarget(GameObject target)
    {
        RaycastHit hit;
        if (target)
            if (Physics.Linecast(transform.position, target.transform.position, out hit))
                return hit.transform.gameObject == target;
        return false;
    }
}
