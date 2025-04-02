using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

//����ÿ�������jumper��root�ϣ�����jumper����Ϊ
//ÿ�����������ͨ��jumper��ququer�����ű�����

//����˵ļ���״̬
public enum JumperState{
    isJumpingIn,//���ڲ����
    isQueuing,//�����Ŷ�
    isBeingGrabbed,//���ڱ�ץס
    isJumpingInAgain,//���ɿ��󷵻ض���
    isJumpingOut//���ɿ����뿪����
}
public class Jumper : MonoBehaviour
{
    [HideInInspector]public JumperState state;//��ǰ״̬

    //is jumping in
    [HideInInspector]public Transform jumpTargetForward, jumpTargetBehind;//�������ʱǰ�ͺ���λ�ã�set in queueController
    int aimI = -1;//���λ������
    Vector3 jumpTargetPos;//���λ��

    // is being grabbed
    public float maxDragDistance = 10f;//��ץס���ߵ������룬�����þ��������ֺ����
    float currentDragDistance;//��ǰ�����ߵľ���

    //is jumping in again or out
    bool flag_recordSpot = true;
    Vector3 returnTargetPos;//���ֺ󷵻ض���λ��
    [HideInInspector] public Transform jumperLeaveTargetPos;//���ֺ��뿪�����Ŀ��λ�ã�set in queueController

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

                //remove from list is in queue controller��in queueController

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

        //������Ըĳ�head��behind���е�λ�ã����ܱ������������
        jumpTargetPos = 0.5f * (jumpTargetBehind.position + jumpTargetForward.position);
        FaceTarget(jumpTargetPos);
        MoveToTarget(jumpTargetPos,0.5f);

        if (flag_reachTargetPos)
        {        
            queuerComponent = gameObject.AddComponent<Queuer>();//�����������λ�ã������queuer���
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
        direction.y = 0; // ����ˮƽ��ת��������б
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
