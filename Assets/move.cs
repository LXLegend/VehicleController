using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public GameObject[] wheelPositions;

    private Rigidbody _rb;

    public float force = 10f;

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
        if (Input.GetKey(KeyCode.W) && _rb)
        {
            foreach (GameObject wheel in wheelPositions)
            {
                _rb.AddForceAtPosition(transform.forward * force, wheel.transform.position, ForceMode.Force);
            }
        }
        if (Input.GetKey(KeyCode.S) && _rb)
        {
            foreach (GameObject wheel in wheelPositions)
            {
                _rb.AddForceAtPosition(-transform.forward * force, wheel.transform.position, ForceMode.Force);
            }
        }

        Debug.Log(transform.position);
    }
}
