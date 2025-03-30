using UnityEngine;
using UnityEngine.AI;

public class Queuer : MonoBehaviour
{
    public Transform target; // ǰһ���Ŷ��߻�Ŀ���
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

            FaceTarget(); // ���ֳ���Ŀ��
        } 
        
        //if is last , check player
        if(isLast)
        {
            DetectPlayerBehind(); // �������߼��
        }
    }

    void DetectPlayerBehind()
    {
        Vector3 rayOrigin = transform.position - transform.forward * 0.5f + transform.up * 1.2f; // �ӱ�����΢ƫ��һ��
        Vector3 rayDirection = -transform.forward; // �����
        float rayDistance = 5f; // ���߳���
        RaycastHit hit;

        //�˴���Ҫ���Ӽ���������������������

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                FindObjectOfType<PlayerQueuer>().GetComponent<PlayerQueuer>().isInQueue = true;
                Debug.Log("������ұ���");
            }
            else
            {
                FindObjectOfType<PlayerQueuer>().GetComponent<PlayerQueuer>().isInQueue = false;
                //Debug.Log("��Ҳ���");
            }
        }
        else
        {
            FindObjectOfType<PlayerQueuer>().GetComponent<PlayerQueuer>().isInQueue = false;
            //Debug.Log("��Ҳ���");
        }

        // �� Scene ��ͼ���ӻ�����
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // ����ˮƽ��ת��������б
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

}