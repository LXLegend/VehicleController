using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public AISeek seekBehaviour;

    public AIVision collisionAvoidance;

    private VehicleController controller;

    // Start is called before the first frame update
    void Start()
    {
        seekBehaviour = GetComponent<AISeek>();

        collisionAvoidance = GetComponent<AIVision>();

        controller = GetComponent<VehicleController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
