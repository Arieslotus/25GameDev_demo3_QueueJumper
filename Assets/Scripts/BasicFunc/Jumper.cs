using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

//挂在每个插队人jumper的root上，控制jumper的行为
//每个插队人身上通过jumper和ququer两个脚本控制

//插队人的几种状态
public enum JumperState{
    isJumpingIn,//正在插入队
    isQueuing,//正在排队
    isBeingGrabbed,//正在被抓住
    isJumpingInAgain,//被松开后返回队伍
    isJumpingOut//被松开后离开队伍
}
public class Jumper : MonoBehaviour
{
    [HideInInspector]public JumperState state;//当前状态

    //is jumping in
    [HideInInspector]public Transform jumpTargetForward, jumpTargetBehind;//插入队伍时前和后人位置，set in queueController
    int aimI = -1;//插队位置序列
    Vector3 jumpTargetPos;//插队位置

    // is being grabbed
    public float maxDragDistance = 10f;//被抓住拖走的最大距离，超过该距离则松手后离队
    float currentDragDistance;//当前被拖走的距离

    //is jumping in again or out
    bool flag_recordSpot = true;
    Vector3 returnTargetPos;//松手后返回队伍位置
    [HideInInspector] public Transform jumperLeaveTargetPos;//松手后离开队伍的目标位置，set in queueController

    bool flag_reachTargetPos = false;

    //component
    Queuer queuerComponent;
    Rigidbody rb;
    private NavMeshAgent agent;
    Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();
        rb = GetComponent<Rigidbody>();
        state = JumperState.isJumpingIn;

        //spawn set
        var list = FindObjectOfType<QueueController>().queue_list;
        if (list.Count < 3)
        {
            Debug.LogError("not enough queue count in jumper controller");
        }
        aimI = Random.Range(1, list.Count - 1); /*edit*/

    }


    void Update()
    {
        switch (state)
        {
            case JumperState.isJumpingIn:

                agent.enabled = true;
                rb.isKinematic = true;
                rb.freezeRotation = true;
                JumpingIntoQueue();

                //Debug.LogError("aaaa");

                break;
            case JumperState.isQueuing:

                //manage in <queuer>
                queuerComponent.enabled = true;
                flag_recordSpot = true;//bug


                //Debug.LogError("bbbb");

                break;
            case JumperState.isBeingGrabbed:// change in playerHoldController
                if (flag_recordSpot)
                {
                    returnTargetPos = gameObject.transform.position;
                    flag_recordSpot = false;
                }
                queuerComponent.enabled = false;
                agent.enabled = false;
                rb.isKinematic = false;
                rb.freezeRotation = false;

                //anima?

                //if dis too far , jump out queue
                currentDragDistance = Vector3.Distance(transform.position, returnTargetPos);
                if(currentDragDistance > maxDragDistance)
                {
                    FindObjectOfType<PlayerHoldController>().jumperOutDistance = true;
                    FindObjectOfType<PlayerHoldController>().flag_canHold = false;
                }

                //Debug.LogError("cccc");

                break;
            case JumperState.isJumpingInAgain:// change in playerHoldController

                agent.enabled = true;
                rb.isKinematic = true;
                rb.freezeRotation = true;
                JumpingIntoQueueAgain();


                //Debug.LogError("dddd");

                break;
            case JumperState.isJumpingOut:// change in playerHoldController

                //remove from list is in queue controller，in queueController

                //move
                agent.enabled = true;
                rb.isKinematic = true;
                rb.freezeRotation = true;
                JumpingOutQueue();

                //Debug.LogError("eeee");

                break;
        }

    }

    void JumpingOutQueue()
    {
        FaceTarget(jumperLeaveTargetPos.position);
        MoveToTarget(jumperLeaveTargetPos.position , 2f);
        if (flag_reachTargetPos)
        {
            //Debug.LogError("delet jumper");
            Destroy(gameObject.transform.parent.gameObject);
            flag_reachTargetPos = false;
        }
    }


    void JumpingIntoQueueAgain()
    {
        FaceTarget(returnTargetPos);
        MoveToTarget(returnTargetPos, 0.5f);
        

        if (flag_reachTargetPos)
        {
            state = JumperState.isQueuing;
            //Debug.LogError("truen sata");

            flag_reachTargetPos = false;
        }
    }

    void JumpingIntoQueue()
    {
        //candelete
        var list = FindObjectOfType<QueueController>().queue_list;
        jumpTargetForward = list[aimI - 1];
        jumpTargetBehind = list[aimI];

        //或许可以改成head和behind的中点位置，就能避免队伍弯曲？
        jumpTargetPos = 0.5f * (jumpTargetBehind.position + jumpTargetForward.position);
        FaceTarget(jumpTargetPos);
        MoveToTarget(jumpTargetPos,0.5f);

        if (flag_reachTargetPos)
        {        
            queuerComponent = gameObject.AddComponent<Queuer>();//如果到达插入队位置，则添加queuer组件
            FindObjectOfType<QueueController>().InsertIntoQueue(transform, aimI);
            state = JumperState.isQueuing;

            flag_reachTargetPos = false;
        }
    }

    private void MoveToTarget(Vector3 targetPos , float stoppedDis)
    {
        float distance = Vector3.Distance(transform.position, targetPos);
        agent.stoppingDistance = stoppedDis;

        if (distance > agent.stoppingDistance)
        {
            agent.SetDestination(targetPos);
            animator.SetBool("isWalk", true);

        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalk", false);

            //reach
            //Debug.LogError("1");
            flag_reachTargetPos = true;
        }
    }
    void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0; // 保持水平旋转，避免倾斜
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
