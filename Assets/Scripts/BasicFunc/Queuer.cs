using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

//����ÿ���Ŷ���queuer��root �� ÿ���ɹ���ӵĲ����jumper��root�ϣ����ڿ����Ŷӵ����Ŷӡ��Զ����桢���������������뿪����Ϊ
public enum QueuerState
{
    isComing,//�������Ŷ�
    isWaiting,//�����Ŷ�
    isLeaving//���������뿪
}
public class Queuer : MonoBehaviour
{
    public bool alreadyQueue = false; /*edit*/ //ÿ����ǰ���ڳ�������Ŷ���true��Ԥ����false
    public QueuerState state;//״̬
    [HideInInspector]public Transform target,behind; //ǰһ���Ŷ��߻�Ŀ��㣬��һ���Ŷ���λ��
    bool flag_reachTargetPos = false;//�Ƿ�ﵽĿ��λ��

    Animator animator;
    private NavMeshAgent agent;

    public bool isLast = false;//�Ƿ��Ƕ�β
    public float minDistance = 1.5f; // �趨��С��ȫ����
    public float maxDistance = 2f; // �趨���ȫ����
    public float retreatSpeed = 1.0f; // �趨���˾���

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
                    DetectPlayerBehind(); // �������߼��
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


    // ���ֳ���Ŀ��
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