using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JumpersController : MonoBehaviour
{
    public GameObject jumperPrefab;
    public Transform spawner1_trans;
    public Transform queueParent_trans;

    public bool flag = false;
    void Start()
    {
        
    }

    
    void Update()
    {
        if (flag)
        {
            flag = false;
            JumpQueue();
            
        }
    }

    public void JumpQueue()
    {
        //�������λ��


        // ��������
        GameObject newJumper = Instantiate(jumperPrefab, spawner1_trans.position, Quaternion.identity, queueParent_trans);


        //// ��ȡ NavMeshAgent ���ƶ���Ŀ���
        //NavMeshAgent agent = newJumper.GetComponent<NavMeshAgent>();
        //if (agent != null)
        //{
        //    agent.SetDestination(targetPosition);
        //}
        //else
        //{
        //    Debug.LogError("���ɵ�����ȱ�� NavMeshAgent �����");
        //}
    }
}
