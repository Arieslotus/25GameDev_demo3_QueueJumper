using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂在玩家每个有碰撞体的肢体上，用于检测玩家是否在地面，从而检测能不能起跳
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
        if (!controller.gameObject.CompareTag("canGrab"))
        {
            controller.isGrounded = true;
        }
        
        //Debug.Log($"{collision.gameObject.name}");
    }
}
