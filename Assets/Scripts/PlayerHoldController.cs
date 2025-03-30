using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoldController : MonoBehaviour
{
    public Animator animator;
    public GameObject grabbedObj;
    bool flag, flag_canHold;
    public Rigidbody handR_rb , handL_rb;

    void Start()
    {
        flag = true;
        flag_canHold = false;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            animator.SetBool("isHold", true);
            flag_canHold = true;


        }
        else if (Input.GetMouseButton(0))
        {

        }
        else if (Input.GetMouseButtonUp(0))
        {

            animator.SetBool("isHold", false);
            flag_canHold = false;

        }

        if (!flag_canHold)
        {
            if (grabbedObj != null)
            {
                Destroy(grabbedObj.GetComponent<FixedJoint>());

                Debug.Log("delet");


            }
            if (grabbedObj != null && grabbedObj.GetComponent<FixedJoint>() == null)
            {
                //make sure no fixed joint , then null
                grabbedObj = null;
                flag = true;
            }

        }
        else
        {
            if (grabbedObj != null && flag)
            {
                FixedJoint jointL = grabbedObj.AddComponent<FixedJoint>();
                jointL.connectedBody = handL_rb;
                jointL.breakForce = 90001;

                FixedJoint jointR = grabbedObj.AddComponent<FixedJoint>();
                jointR.connectedBody = handR_rb;
                jointR.breakForce = 90001;

                flag = false;
            }
        }




    }

}
