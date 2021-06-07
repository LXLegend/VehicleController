using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleController))]
public class VehicleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        VehicleController vehicle = (VehicleController) target;


        foreach (WheelScript wheel in vehicle.frontWheels)
        {
            wheel.wheelObject.transform.localScale = new Vector3(vehicle.frontWheelWidth, vehicle.frontWheelDiameter, vehicle.frontWheelDiameter);
            // wheel.maxDisplacement = wheel.transform.localScale.y;
            wheel.wheelObject.transform.localPosition = new Vector3(0, -wheel.springRestLength, 0);
            wheel.transform.localPosition = new Vector3(wheel.transform.localPosition.x, vehicle.frontWheelAnchorHeight, vehicle.wheelBaseLength / 2);
                  
            wheel.maxSpringLength = wheel.springRestLength + wheel.maxSpringTravel;
            wheel.minSpringLength = wheel.springRestLength - wheel.maxSpringTravel;
            wheel.raycastlength = wheel.maxSpringLength + vehicle.frontWheelDiameter;
            if (wheel.frontLeft)
                wheel.transform.localPosition = new Vector3( - vehicle.frontWheelDistance / 2, wheel.transform.localPosition.y, wheel.transform.localPosition.z);
            else if (wheel.frontRight)
                wheel.transform.localPosition = new Vector3(vehicle.frontWheelDistance / 2, wheel.transform.localPosition.y, wheel.transform.localPosition.z);
        }

        foreach (WheelScript wheel in vehicle.backWheels)
        {
            wheel.wheelObject.transform.localScale = new Vector3(vehicle.backWheelWidth, vehicle.backWheelDiameter, vehicle.backWheelDiameter);
            // wheel.maxDisplacement = wheel.transform.localScale.y;
            wheel.wheelObject.transform.localPosition = new Vector3(0, -wheel.springRestLength, 0);
            wheel.transform.localPosition = new Vector3(wheel.transform.localPosition.x, vehicle.backWheelAnchorHeight, -vehicle.wheelBaseLength / 2);
            wheel.maxSpringLength = wheel.springRestLength + wheel.maxSpringTravel;
            wheel.minSpringLength = wheel.springRestLength - wheel.maxSpringTravel;
            wheel.raycastlength = wheel.maxSpringLength + vehicle.backWheelDiameter;
            if (wheel.backLeft)
                wheel.transform.localPosition = new Vector3( - vehicle.backWheelDistance / 2, wheel.transform.localPosition.y, wheel.transform.localPosition.z);
            else if (wheel.backRight)
                wheel.transform.localPosition = new Vector3(vehicle.backWheelDistance / 2, wheel.transform.localPosition.y, wheel.transform.localPosition.z);
        }
    }
}
