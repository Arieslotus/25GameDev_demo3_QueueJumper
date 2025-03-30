using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    private PlayerController controller;
    void Start()
    {
        controller = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
    }



    private void OnTriggerStay(Collider other)
    {
        if (controller.gameObject.CompareTag("ground"))
        {

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        controller.isGrounded = true;
        //Debug.Log($"{collision.gameObject.name}");
    }
}
