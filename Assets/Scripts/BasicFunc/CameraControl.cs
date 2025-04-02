using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于镜头控制，建议优化镜头控制方式？

public class CameraControl : MonoBehaviour
{
    
    public float rotationSpeed = 1.0f;
    //public Transform root;
    //public ConfigurableJoint hipJoint,stomachJoint;

    //float mouseX, mouseY;

    public float XSpeed = 300f;
    public float YSpeed = 2f;

    CinemachineFreeLook mFreeLook;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        mFreeLook = GetComponent<CinemachineFreeLook>();
    }


    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            mFreeLook.m_XAxis.m_MaxSpeed = XSpeed;
            mFreeLook.m_YAxis.m_MaxSpeed = YSpeed;
        }
        else
        {
            mFreeLook.m_XAxis.m_MaxSpeed = 0;
            mFreeLook.m_YAxis.m_MaxSpeed = 0;
        }
    }


    //not use
    //void CamControl()
    //{
    //    mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
    //    mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
    //    mouseY = Mathf.Clamp(mouseY, -35, 60);

    //    Quaternion rootRotation = Quaternion.Euler(mouseX, mouseY, 0);

    //    root.rotation = rootRotation;

    //    hipJoint.targetRotation = Quaternion.Euler(0 , -mouseX, 0);

    //}
}
