using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (Input.GetMouseButtonDown(0))
        {
            flag_canHold = true;


        }
        else if (Input.GetMouseButton(0))
        {

        }
        else if (Input.GetMouseButtonUp(0))
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
            
            //if (holdController.grabbedObj != null && flag)
            //{



            //    flag = false;
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //grabbedObj = null;
    }
}
