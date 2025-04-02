using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂在玩家前臂上，用于向holdController发送抓取物体消息
public class Hand : MonoBehaviour
{   
    PlayerHoldController holdController;
    bool flag_canHold = false;

    void Start()
    {

    }


    void Update()
    {
        if(holdController == null)
        {
            holdController = FindObjectOfType<PlayerHoldController>();
        }

        if (Input.GetMouseButtonDown(2))
        {
            flag_canHold = true;


        }
        else if (Input.GetMouseButton(2))
        {

        }
        else if (Input.GetMouseButtonUp(2))
        {
            flag_canHold = false;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("canGrab") && flag_canHold)
        {
            if(holdController.grabbedObj == null)
            {
                holdController.grabbedObj = other.gameObject;

            }
        }
        else if(other.gameObject.CompareTag("jumper") && flag_canHold)
        {
            if (holdController.grabbedObj == null)
            {
                holdController.grabbedObj = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //grabbedObj = null;
    }
}
