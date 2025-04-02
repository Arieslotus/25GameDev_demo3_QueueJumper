using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//������ҵ�hip�������ϣ���������ƶ�����Ծ�����١��������
//����޸ľ�ͷ���ƣ�����ͬʱ�����ҿ���

public class PlayerController : MonoBehaviour
{
    public float speed; //�ƶ��ٶ�
    public float jumpForce; //��Ծ�߶�
    //public float rotationSpeed = 10f; // ��ת�ٶ�
    public ConfigurableJoint hipJoints; //hip��configureJoint
    Vector3 targetRot = Vector3.zero; //������ת�Ƕ�
    Vector3 moveDirection = Vector3.zero;//�ƶ���������

    public Rigidbody hips;//hip��rb
    [HideInInspector]public bool isGrounded;//����Ƿ�Ӵ�����

    public Animator animator;//���ƶ�����ģ�͵��Ǹ�animator


    void Start()
    {
        hips = GetComponent<Rigidbody>();
        //Cursor.lockState = CursorLockMode.Locked;
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

    void WalkAndRun(KeyCode key , Vector3 aimMoveDir)
    {
        if (Input.GetKey(key))
        {
            moveDirection += aimMoveDir;
            //moveDirection += hips.transform.forward;

            //anima
            animator.SetBool("isWalk", true);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDirection *= 1.5f; // �ٶȱ���
                animator.SetBool("isRun", true);
            }
        }

    }
    private void FixedUpdate()
    {
        //rote
        hipJoints.targetRotation = Quaternion.Euler(targetRot);

        //move
        if (!isGrounded) return;

        //��Ҿ�ͷ���ƶ����Ƶ���զ���ƣ�
        moveDirection = Vector3.zero;
        WalkAndRun(KeyCode.W , Vector3.forward); //Vector3.forward
        WalkAndRun(KeyCode.A , -Vector3.right);
        WalkAndRun(KeyCode.S , -Vector3.forward);
        WalkAndRun(KeyCode.D , Vector3.right);

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            //anima
            animator.SetBool("isWalk", false);
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            //anima
            animator.SetBool("isRun", false);
        }

        
        // ��һ�����򣬱���Խ����ٶȱ��
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();

            // **�ý�ɫ�����ƶ�����**
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            targetRot = new Vector3(targetRotation.eulerAngles.x, -targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);
            //root.rotation = Quaternion.Slerp(root.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            

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
