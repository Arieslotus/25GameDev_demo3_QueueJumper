using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������hip�ϣ����ڿ��������Ϊ�Ŷ���֮һʱ�Ĺ���

public class PlayerQueuer : MonoBehaviour
{
    public bool isInQueue = false;//����Ƿ��ڶ���
    bool flag = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isInQueue)
        {
            if (flag)
            {
                FindObjectOfType<QueueController>().GetComponent<QueueController>().AddToQueue(this.transform);//�ڶ��������list
                flag = false;
            }      
        }
        else
        {
            if(!flag)
            {
                FindObjectOfType<QueueController>().GetComponent<QueueController>().RemoveFromQueue(this.transform);//�������list�Ƴ�
                flag = true;
            }
            
        }
    }
}
