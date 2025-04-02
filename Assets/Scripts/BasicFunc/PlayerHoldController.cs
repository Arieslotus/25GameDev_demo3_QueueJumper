using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂在玩家hip上，用于控制被抓住的物体相关，统一管理从两个手hand组件传来的消息

public class PlayerHoldController : MonoBehaviour
{
    public Animator animator;//复制动画的模型的animator
    public GameObject grabbedObj;//当前抓住的物体（自动设置）
    bool flag;
    public bool flag_canHold;//当前是否能抓东西，不能抓东西的两种情况：1松开键，2jumper距离过远自动脱手
    [HideInInspector]public bool jumperOutDistance = false;//当前抓的jumper是否距离过远，set in jumpers
    public Rigidbody handR_rb , handL_rb;//左右手的rb

    void Start()
    {
        flag = true;
        flag_canHold = false;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(2))//目前是鼠标中键，按住抓取，可优化？
        {

            animator.SetBool("isHold", true);
            flag_canHold = true;


        }
        else if (Input.GetMouseButton(2))
        {

        }
        else if (Input.GetMouseButtonUp(2))
        {

            animator.SetBool("isHold", false);
            flag_canHold = false;

        }

        if (!flag_canHold)
        {
            LetGo();
        }
        else
        {
            if (grabbedObj != null)
            {
                StartGrab();
            }
        }
    }

    void StartGrab()
    {
        if (flag)
        {
            //设置被抓物和两个手的连接点
            //可优化ik？
            FixedJoint jointL = grabbedObj.AddComponent<FixedJoint>();
            jointL.connectedBody = handL_rb;
            jointL.breakForce = 900;//不要太大，不然会碰撞体穿插抽搐
            
            FixedJoint jointR = grabbedObj.AddComponent<FixedJoint>();
            jointR.connectedBody = handR_rb;
            jointR.breakForce = 900;

            //如果抓到jumper
            if (grabbedObj.tag == "jumper")
            {
                Debug.LogWarning($"grab: {grabbedObj.name}");

                //获取父物体中的jumper组件，设置jumper状态为被抓
                Transform current = grabbedObj.transform;
                var jumper = current.GetComponent<Jumper>();
                while (current != null)
                {
                    jumper = current.GetComponent<Jumper>();
                    if (jumper != null)
                    {
                        jumper.state = JumperState.isBeingGrabbed;
                        break; // 找到后直接返回
                    }
                    current = current.parent; // 继续向上遍历
                }
                if (jumper == null)
                {
                    Debug.LogError("jumper tag but no Jumper component found in any parent.");
                }
            }

            flag = false;//控制函数执行一次

            jumperOutDistance = false;//bug
        }
        
    }

    void LetGo()
    {
        if (grabbedObj != null)
        {
            //删除连接点（由于连接点不止一个，这个函数可能调用两次）
            //Debug.LogError("delet");
            Destroy(grabbedObj.GetComponent<FixedJoint>());

            //如果连接点都删完了
            if (grabbedObj.GetComponent<FixedJoint>() == null)
            {
                //如果抓的是jumper，松手时需要根据是否超过最远距离，获取父物体jumper组件，设置jumper状态
                if (grabbedObj.tag == "jumper")
                {
                    Transform current = grabbedObj.transform;
                    var jumper = current.GetComponent<Jumper>();

                    while (current != null)
                    {
                        jumper = current.GetComponent<Jumper>();
                        if (jumper != null)
                        {
                            if (jumperOutDistance)
                            {
                                jumper.state = JumperState.isJumpingOut;//超距离则设置jumper为离队
                                //Debug.LogError("hold ser jump out");
                            }
                            else
                            {
                                jumper.state = JumperState.isJumpingInAgain;//未超距离则设置jumper回队
                                //Debug.LogError("hold set jump in");
                            }                     
                            break; // 找到后直接返回
                        }
                        current = current.parent; // 继续向上遍历父物体
                    }

                    if (jumper == null)
                    {
                        Debug.LogError("jumper tag but no Jumper component found in any parent.");
                    }

                }

                //确认删完所有连接点，且设置好jumper状态，则设置抓物体为空
                grabbedObj = null;
                flag = true;
                
            }

        }
    }

}
