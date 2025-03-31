using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Queuer : MonoBehaviour
{
    public Transform target,behind; // 前一个排队者或目标点
    private NavMeshAgent agent;

    Animator animator;

    public bool isLast = false;
    public float minDistance = 1.5f; // 设定最小安全距离
    public float maxDistance = 2f; // 设定最小安全距离
    public float retreatSpeed = 1.0f; // 设定后退距离

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        //move
        if (target != null)
        {
            MoveToTarget();
            FaceTarget(); // 保持朝向目标

        } 
        
        //check dis with front


        //if is last , check player
        if(isLast)
        {
            DetectPlayerBehind(); // 运行射线检测
        }
    }

    private void MoveToTarget()
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