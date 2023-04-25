using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringTest : MonoBehaviour
{
    // spring Length
    public float springLength = 2f;

    // the k value of the spring
    public float springStiffness = 10f;

    // the previous displacement of the spring
    private float prevDisplacement = 0f;

    // the current displacement of the spring
    private float currentDisplacement = 0f;

    // the dampening amount
    public float dampeningAmount = 10f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, springLength))
        {
            prevDisplacement = currentDisplacement;
            currentDisplacement = Mathf.Max(springLength - raycastHit.distance, 0);
        }
        Debug.Log(calculateSpringForce());
    }

    public float calculateDisplacementVelocity(float deltaTime)
    {
        return (currentDisplacement - prevDisplacement) / deltaTime;
    }

    public float calculateSpringForce()
    {
        float springForce = springStiffness * currentDisplacement;
        float damperForce = dampeningAmount * calculateDisplacementVelocity(Time.fixedDeltaTime);

        return springForce - damperForce;
    }
}
