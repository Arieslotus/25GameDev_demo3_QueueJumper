using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//挂在JUMPER空物体上，用于管理插队者jumpers生成

public class JumpersController : MonoBehaviour
{
    public GameObject jumperPrefab;//jumper预制体
    public Transform spawner1_trans;//jumper出生点1，可以优化更多出生点？
    public Transform queueParent_trans;//队列父物体QUEUE

    public bool flag_spawnJumper = false;//生成1个jumper
    void Start()
    {
        flag_spawnJumper = true;
    }

    
    void Update()
    {
        if (flag_spawnJumper)
        {
            flag_spawnJumper = false;
            JumpQueue();
            
        }
    }

    public void JumpQueue()
    {
        //随机生成位置

        // 生成人物
        GameObject newJumper = Instantiate(jumperPrefab, spawner1_trans.position, Quaternion.identity, queueParent_trans);

    }
}
