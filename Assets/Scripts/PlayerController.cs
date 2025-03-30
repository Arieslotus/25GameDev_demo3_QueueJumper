using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float strafeSpeed;
    public float jumpForce;
    public float rotationSpeed = 10f; // ��ת�ٶ�

    public Rigidbody hips;
    public bool isGrounded;

    public Animator animator;
    void Start()
    {
        hips = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //jump
        if (!isGrounded)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("addf");
            if (isGrounded)
            {
                hips.AddForce(new Vector3(0, jumpForce, 0));
                isGrounded = false;

            }
        }
    }
    private void FixedUpdate()
    {
        if (!isGrounded) return;

        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += hips.transform.forward;
            animator.SetBool("isWalk", true);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDirection *= 1.5f; // �ٶȱ���
                animator.SetBool("isRun", true);
            }

        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isRun", false);
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= hips.transform.right;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= hips.transform.forward;
            animator.SetBool("isWalk", true);
        }
        else if (!Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isWalk", false);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += hips.transform.right;
        }

        // ��һ�����򣬱���Խ����ٶȱ��
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();

            // **�ý�ɫ�����ƶ�����**
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
           // hips.rotation = Quaternion.Slerp(hips.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            // **�ƶ�**
            hips.velocity = new Vector3(moveDirection.x * speed, hips.velocity.y, moveDirection.z * speed);
        }
        else
        {
            // ֹͣʱֻ������ֱ�ٶ�
            hips.velocity = new Vector3(0, hips.velocity.y, 0);
        }
    }


}
