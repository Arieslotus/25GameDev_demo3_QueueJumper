using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//����JUMPER�������ϣ����ڹ�������jumpers����

public class JumpersController : MonoBehaviour
{
    public GameObject jumperPrefab;//jumperԤ����
    public Transform spawner1_trans;//jumper������1�������Ż���������㣿
    public Transform queueParent_trans;//���и�����QUEUE

    public bool flag_spawnJumper = false;//����1��jumper
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
        //�������λ��

        // ��������
        GameObject newJumper = Instantiate(jumperPrefab, spawner1_trans.position, Quaternion.identity, queueParent_trans);

    }
}
