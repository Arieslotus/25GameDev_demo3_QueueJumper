using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public Transform root;
    public ConfigurableJoint hipJoint,stomachJoint;

    float mouseX, mouseY;

    public float stomachOffset;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CamControl();
    }

    void CamControl()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        Quaternion rootRotation = Quaternion.Euler(mouseX, mouseY, 0);

        root.rotation = rootRotation;

        hipJoint.targetRotation = Quaternion.Euler(0 , -mouseX, 0);
        stomachJoint.targetRotation = Quaternion.Euler(-mouseY * stomachOffset, 0 , 0);

    }
}
