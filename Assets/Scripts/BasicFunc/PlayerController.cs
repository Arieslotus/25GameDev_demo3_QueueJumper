using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//挂在玩家的hip子物体上，用于玩家移动、跳跃、加速、朝向控制
//如果修改镜头控制，建议同时检查玩家控制

public class PlayerController : MonoBehaviour
{
    public float speed; //移动速度
    public float jumpForce; //跳跃高度
    //public float rotationSpeed = 10f; // 旋转速度
    public ConfigurableJoint hipJoints; //hip的configureJoint
    Vector3 targetRot = Vector3.zero; //朝向旋转角度
    Vector3 moveDirection = Vector3.zero;//移动方向向量

    public Rigidbody hips;//hip的rb
    [HideInInspector]public bool isGrounded;//玩家是否接触地面

    public Animator animator;//复制动作的模型的那个animator


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
                moveDirection *= 1.5f; // 速度倍率
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

        //玩家镜头和移动控制到底咋控制？
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

        
        // 归一化方向，避免对角线速度变快
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();

            // **让角色朝向移动方向**
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            targetRot = new Vector3(targetRotation.eulerAngles.x, -targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);
            //root.rotation = Quaternion.Slerp(root.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            

            // **移动**
            hips.velocity = new Vector3(moveDirection.x * speed, hips.velocity.y, moveDirection.z * speed);
        }
        else
        {
            // 停止时只保留垂直速度
            hips.velocity = new Vector3(0, hips.velocity.y, 0);
        }


    }


}
