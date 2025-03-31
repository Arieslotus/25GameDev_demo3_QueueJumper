using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Queuer : MonoBehaviour
{
    public Transform target,behind; // ǰһ���Ŷ��߻�Ŀ���
    private NavMeshAgent agent;

    Animator animator;

    public bool isLast = false;
    public float minDistance = 1.5f; // �趨��С��ȫ����
    public float maxDistance = 2f; // �趨��С��ȫ����
    public float retreatSpeed = 1.0f; // �趨���˾���

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
            FaceTarget(); // ���ֳ���Ŀ��

        } 
        
        //check dis with front


        //if is last , check player
        if(isLast)
        {
            DetectPlayerBehind(); // �������߼��
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