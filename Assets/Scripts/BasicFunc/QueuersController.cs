using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂在QUEUER空物体上，用于控制来队尾排队的人的生成
public class QueuersController : MonoBehaviour
{
    public GameObject queuerPrefab;//排队的人预制体
    public Transform spawner1_trans;//生成位置1
    public Transform queueParent_trans;//队伍QUEUE父物体
    public bool flag_spawnQueuer = false;//生成1个来队尾排队的人，change in inspector by hand
    void Start()
    {

    }


    void Update()
    {
        if (flag_spawnQueuer)
        {
            flag_spawnQueuer = false;
            JumpQueue();

        }
    }

    public void JumpQueue()
    {
        //随机生成位置


        // 生成人物
        GameObject newJumper = Instantiate(queuerPrefab, spawner1_trans.position, Quaternion.identity, queueParent_trans);

    }
}
