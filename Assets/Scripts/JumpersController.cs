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
        //随机生成位置


        // 生成人物
        GameObject newJumper = Instantiate(jumperPrefab, spawner1_trans.position, Quaternion.identity, queueParent_trans);


        //// 获取 NavMeshAgent 并移动到目标点
        //NavMeshAgent agent = newJumper.GetComponent<NavMeshAgent>();
        //if (agent != null)
        //{
        //    agent.SetDestination(targetPosition);
        //}
        //else
        //{
        //    Debug.LogError("生成的人物缺少 NavMeshAgent 组件！");
        //}
    }
}
