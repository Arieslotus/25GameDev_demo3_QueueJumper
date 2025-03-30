using UnityEngine;
using UnityEngine.AI;

public class Queuer : MonoBehaviour
{
    public Transform target; // 前一个排队者或目标点
    private NavMeshAgent agent;

    Animator animator;

    public bool isLast = false;

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
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > agent.stoppingDistance)
            {
                agent.SetDestination(target.position);
                animator.SetBool("isWalk", true);
                
            }
            else
            {
                agent.ResetPath();
                animator.SetBool("isWalk", false);
            }

            FaceTarget(); // 保持朝向目标
        } 
        
        //if is last , check player
        if(isLast)
        {
            DetectPlayerBehind(); // 运行射线检测
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