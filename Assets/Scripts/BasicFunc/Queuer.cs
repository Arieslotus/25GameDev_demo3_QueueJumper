using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

//挂在每个排队者queuer的root 和 每个成功入队的插队者jumper的root上，用于控制排队的来排队、自动跟随、过近撤步、买完离开的行为
public enum QueuerState
{
    isComing,//正在来排队
    isWaiting,//正在排队
    isLeaving//正在买完离开
}
public class Queuer : MonoBehaviour
{
    public bool alreadyQueue = false; /*edit*/ //每个提前放在场景里的排队者true，预制体false
    public QueuerState state;//状态
    [HideInInspector]public Transform target,behind; //前一个排队者或目标点，后一个排队者位置
    bool flag_reachTargetPos = false;//是否达到目标位置

    Animator animator;
    private NavMeshAgent agent;

    public bool isLast = false;//是否是队尾
    public float minDistance = 1.5f; // 设定最小安全距离
    public float maxDistance = 2f; // 设定最大安全距离
    public float retreatSpeed = 1.0f; // 设定后退距离

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();

        if(alreadyQueue )
        {
            state = QueuerState.isWaiting;//people already in scene queue
        }
        else
        {
            if(GetComponent<Jumper>() != null)
            {
                state = QueuerState.isWaiting;//people jump in queue
            }
            else
            {
                state = QueuerState.isComing;//people who will be instancitaed and add to queue end

                //spawn set
                var list = FindObjectOfType<QueueController>().queue_list;
                target = list[list.Count - 1];
                behind = null;
            }

        }
        
    }

    void Update()
    {
        switch (state)
        {
            case QueuerState.isComing:
                //move
                if (target != null)
                {
                    MoveToQueueEnd();
                    FaceTarget();
                }
                
                break;

            case QueuerState.isWaiting:
                //move
                if (target != null)
                {
                    MoveToQueuer();
                    FaceTarget();
                }
                //if is last , check player
                if (isLast)
                {
                    DetectPlayerBehind(); // 运行射线检测
                }

                break; 
            
            case QueuerState.isLeaving://change in queueController
                //move
                if(target != null)
                {
                    MoveToLeaveSpot();
                    FaceTarget();
                }

                break;

        }

    }

    private void MoveToQueueEnd()
    {
        MoveToTarget();

        if (flag_reachTargetPos)
        {
            FindObjectOfType<QueueController>().AddToQueue(transform);
            state = QueuerState.isWaiting;
            flag_reachTargetPos = false;
        }
    }
    private void MoveToLeaveSpot()
    {
        MoveToTarget();

        if(flag_reachTargetPos)
        {
            Destroy(gameObject.transform.parent.gameObject);
            flag_reachTargetPos = false;
        }
    }

    void MoveToTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > maxDistance)
        {
            agent.SetDestination(target.position);
            animator.SetBool("isWalk", true);
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalk", false);

            flag_reachTargetPos = true;
        }
        
    }

    private void MoveToQueuer()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > maxDistance)//agent.stoppingDistance
        {
            agent.SetDestination(target.position);
            animator.SetBool("isWalk", true);

        }
        else if (distance > minDistance)
        {
            agent.ResetPath();
            animator.SetBool("isWalk", false);

        }
        else
        {
            //check distance with front
            Vector3 retreatDirection = Vector3.zero;
            if (!isLast)
            {
                retreatDirection = (behind.position - target.position).normalized;
            }
            else
            {
                retreatDirection = (transform.position - target.position).normalized;
            }
            
            Vector3 retreatPosition = retreatDirection * retreatSpeed * Time.deltaTime;
            //Vector3 retreatPosition = transform.position + retreatDirection * retreatDistance;
            //agent.SetDestination(retreatPosition);
            transform.position += retreatPosition;

            animator.SetBool("isWalk", true);

            //Debug.Log("...");
        }
    }

    void DetectPlayerBehind()
    {
        Vector3 rayOrigin = transform.position - transform.forward * 0.5f + transform.up * 1.2f; // 从背后稍微偏移一点
        Vector3 rayDirection = -transform.forward; // 向后发射
        float rayDistance = 5f; // 射线长度
        RaycastHit hit;

        //此处需要增加检测的射线数量和扇形区域

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                FindObjectOfType<PlayerQueuer>().GetComponent<PlayerQueuer>().isInQueue = true;
                Debug.Log("玩家在我背后！");
            }
            else
            {
                FindObjectOfType<PlayerQueuer>().GetComponent<PlayerQueuer>().isInQueue = false;
                //Debug.Log("玩家不在");
            }
        }
        else
        {
            FindObjectOfType<PlayerQueuer>().GetComponent<PlayerQueuer>().isInQueue = false;
            //Debug.Log("玩家不在");
        }

        // 在 Scene 视图可视化射线
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);
    }


    // 保持朝向目标
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // 保持水平旋转，避免倾斜
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

}