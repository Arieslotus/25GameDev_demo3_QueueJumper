using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    public Animator animator;
    public GameObject grabbedObj;
    bool flag;
    private Rigidbody rb;
    public int isLeftorRight;
    public bool alreadyHolding;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        flag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(isLeftorRight))
        {
            if(isLeftorRight == 0)
            {
                animator.SetBool("isLHold", true);
            }
            else if(isLeftorRight == 1)
            {
                animator.SetBool("isRHold", true);
            }




        }
        else if (Input.GetMouseButton(isLeftorRight))
        {
            if (grabbedObj != null && flag)
            {
                
                FixedJoint joint = grabbedObj.AddComponent<FixedJoint>();
                joint.connectedBody = rb;
                joint.breakForce = 90001;

                flag = false;
            }
        }
        else if (Input.GetMouseButtonUp(isLeftorRight))
        {
            if (isLeftorRight == 0)
            {
                animator.SetBool("isLHold", false);
            }
            else if (isLeftorRight == 1)
            {
                animator.SetBool("isRHold", false);
            }


        }
        else
        {
            if (grabbedObj != null)
            {
                Destroy(grabbedObj.GetComponent<FixedJoint>());
                flag = true;
                Debug.Log("delet");

            }
            grabbedObj = null;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("canGrab"))
        {
            grabbedObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grabbedObj = null;
    }
}
