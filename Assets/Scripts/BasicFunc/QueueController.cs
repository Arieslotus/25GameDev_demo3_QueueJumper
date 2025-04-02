using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//挂在QUEUE上，用于管理队列内所有人

public class QueueController : MonoBehaviour
{
    public List<Transform> queue_list = new List<Transform>();//所有队列里的人

    int lastQueuerId = -1;//队列里最后一个排队者的序号（不包括玩家）

    public bool flag_firstQueuerLeave = false;//让第一个人买完离队，change in inspector by hand

    int grabbedJumperId = -1;//当前抓到的jumper的序号
    public bool isPaused = false; //队伍是否暂停移动

    public Transform queuerLeaveSpot1_trans;//排队者买完东西离开的目标位置
    public Transform jumperLeaveSpot1_trans;//插队者被拽走后离队的目标位置

    void Awake()
    {
        //获取所有子物体到放到队列
        GetChildrenRootTransforms();
    }

    void Update()
    {
        //管理队列
        ManageQueue();

        //队伍暂停 或 队伍恢复 的控制
        if(grabbedJumperId != -1)
        {      
            if (isPaused)
            {           
                queue_list[grabbedJumperId + 1].GetComponent<Queuer>().enabled = false;
                queue_list[grabbedJumperId + 1].GetComponent<NavMeshAgent>().enabled = false;                
            }
            else
            {
                //Debug.LogError("eee0");

                //可优化，不要每帧设置？
                //queue_list[grabbedJumperId].GetComponent<Queuer>().enabled = true;
                //queue_list[grabbedJumperId].GetComponent<NavMeshAgent>().enabled = true;
                queue_list[grabbedJumperId + 1].GetComponent<Queuer>().enabled = true;
                queue_list[grabbedJumperId + 1].GetComponent<NavMeshAgent>().enabled = true;
            }
        }


        //第一个人买完离队
        if (flag_firstQueuerLeave)
        {
            flag_firstQueuerLeave = false;
            Buy();
        }
    }

    void Buy()
    {
        int id = 1;
        var queuer = queue_list[id].GetComponent<Queuer>();
        if (queuer != null)
        {
            queuer.state = QueuerState.isLeaving;
            queuer.target = queuerLeaveSpot1_trans;//优化不同位置
            queuer.behind = null;
        }
        queue_list.Remove(queue_list[id]);

        //queuer行为在queuer脚本里控制
    }

    void ManageQueue()
    {
        isPaused = false;

        //遍历每个队列里的人
        for (int i = 0; i < queue_list.Count; i++)
        {
            //按照顺序重命名
            RenameObject(queue_list[i] , i);

            //对于 除了head、除了最后一个（不论是玩家还是排队者） 的人
            if(i > 0 && i < queue_list.Count -1)
            {
                //queuer-------------------------------------------------------
                var queuer = queue_list[i].GetComponent<Queuer>();
                //Debug.Log($"{queuer.name} + {queuer.enabled}");
                
                if ( queuer != null)
                {
                    if (queuer.enabled)
                    {
                        //queuer
                        queuer.target = queue_list[i - 1].transform;
                        queuer.behind = queue_list[i + 1].transform;
                        queuer.isLast = false;
                    }
                    else
                    {
                        Debug.Log($"{queuer.name} + not enable");
                    }
                }
                else
                {
                    Debug.LogError($"{queuer.name} : no queuer fond in list in ququw contrillwe");
                }

                //jumper-------------------------------------------------------

                var jumper = queue_list[i].GetComponent<Jumper>();
                if ( jumper != null && jumper.enabled)
                {
                    //如果存在jumper是被抓或正归队状态，队列暂停
                    if(jumper.state == JumperState.isBeingGrabbed || jumper.state == JumperState.isJumpingInAgain)
                    {
                        isPaused = true;
                        grabbedJumperId = i;

                        //Debug.LogError("pause");
                    }
                    //如果存在jumper因被抓距离过远离队，jumper移出队列
                    else if(jumper.state == JumperState.isJumpingOut)
                    {         
                        RemoveFromQueue(queue_list[i]);
                        //isPaused = false;

                        grabbedJumperId = i - 1;//bug

                        //Debug.LogError("jump out");
                    }
                    else
                    {
                        //grabbedJumperId = -1;
                    }

                    //统一设置好每个jumper如果因被抓距离过远离队 的 目标位置
                    if(jumper.jumperLeaveTargetPos == null)
                    {
                        jumper.jumperLeaveTargetPos = jumperLeaveSpot1_trans;//优化不同位置
                    }
                }


            }

            //对于最后一个（不论是玩家还是排队者）人
            if (i == queue_list.Count - 1)
            {
                var queuer = queue_list[i].GetComponent<Queuer>();
                var playerQueuer = queue_list[i].GetComponent<PlayerQueuer>();
                if (playerQueuer != null && queuer == null)
                {
                    //player in queue
                    lastQueuerId = i - 1;

                }
                else if(queuer != null && playerQueuer ==null)
                {
                    //player out queue
                    queuer.target = queue_list[i - 1].transform;
                    queuer.behind = null;
                    lastQueuerId = i;
                }
                else
                {
                    Debug.LogError("no queuer fond in list in ququw contrillwe");
                }

                queue_list[lastQueuerId].GetComponent<Queuer>().isLast = true;
            }
        }

        
    }

    public void RenameObject( Transform obj , int number)
    {
        string currentName = obj.name;
        string formattedNumber = number.ToString("D3"); // 转换为三位数格式

        // 检测名称最后一个字符是否为数字
        if (char.IsDigit(currentName.Last()))
        {
            // 替换最后的数字为新的三位数
            int lastDigitIndex = currentName.Length - 1;
            while (lastDigitIndex > 0 && char.IsDigit(currentName[lastDigitIndex - 1]))
            {
                lastDigitIndex--;
            }
            obj.name = currentName.Substring(0, lastDigitIndex) + formattedNumber;
        }
        else
        {
            // 在名称后添加三位数
            obj.name = currentName + formattedNumber;
        }

    }

    void GetChildrenRootTransforms()
    {
        queue_list.Clear();

        foreach (Transform child in transform)
        {
            if(child.name == "head")
            {
                //queue_list.Add(child);
                queue_list.Insert(0, child);
            }
            else
            {
                if (child.childCount > 0)
                {
                    queue_list.Add(child.GetChild(1));

                }
            }

        }
    }

    public void AddToQueue(Transform transform)
    {
        //add player in last
        queue_list.Add(transform);
    }

    public void InsertIntoQueue(Transform transform , int i)
    {
        if (!queue_list.Contains(transform))
        {
            if( 0 <= i &&  i < queue_list.Count)
            {
                queue_list.Insert(i, transform);
            }
            else
            {
                Debug.LogError("worong insert in queue controller");
            }
        }
        
    }

    public void RemoveFromQueue(Transform transform) {  queue_list.Remove(transform); }
}
