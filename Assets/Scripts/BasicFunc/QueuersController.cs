using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����QUEUER�������ϣ����ڿ�������β�Ŷӵ��˵�����
public class QueuersController : MonoBehaviour
{
    public GameObject queuerPrefab;//�Ŷӵ���Ԥ����
    public Transform spawner1_trans;//����λ��1
    public Transform queueParent_trans;//����QUEUE������
    public bool flag_spawnQueuer = false;//����1������β�Ŷӵ��ˣ�change in inspector by hand
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
        //�������λ��


        // ��������
        GameObject newJumper = Instantiate(queuerPrefab, spawner1_trans.position, Quaternion.identity, queueParent_trans);

    }
}
