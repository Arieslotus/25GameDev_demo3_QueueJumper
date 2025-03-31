using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Jumper : MonoBehaviour
{
    public bool isJumping = true;
    public Transform jumpTargetForward, jumpTargetBehind;
    public int aimI = -1;
    Vector3 jumpTargetPos;


    private NavMeshAgent agent;

    Animator animator;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();

        var list = FindObjectOfType<QueueController>().queue_list;
        if (list.Count < 3)
        {
            Debug.LogError("not enough queue count in jumper controller");
        }
        aimI = Random.Range(1, list.Count - 1); /*edit*/

    }


    void Update()
    {
        if (isJumping)
        {
            JumpingIntoQueue();
        }
    }

    void JumpingIntoQueue()
    {
        //candelete
        var list = FindObjectOfType<QueueController>().queue_list;
        jumpTargetForward = list[aimI - 1];
        jumpTargetBehind = list[aimI];

        jumpTargetPos = 0.5f * (jumpTargetBehind.position + jumpTargetForward.position);
        FaceTarget();
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        float distance = Vector3.Distance(transform.position, jumpTargetPos);
        agent.stoppingDistance = 0.5f;

        if (distance > agent.stoppingDistance)
        {
            agent.SetDestination(jumpTargetPos);
            animator.SetBool("isWalk", true);

        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalk", false);

            //reach
            //Debug.LogError("1");

            gameObject.AddComponent<Queuer>();
            FindObjectOfType<QueueController>().InsertIntoQueue(transform , aimI);
            isJumping = false;

        }
    }
    void FaceTarget()
    {
        Vector3 direction = (jumpTargetPos - transform.position).normalized;
        direction.y = 0; // 保持水平旋转，避免倾斜
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
