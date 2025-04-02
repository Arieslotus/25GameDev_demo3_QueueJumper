using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂在玩家hip上，用于控制玩家作为排队人之一时的功能

public class PlayerQueuer : MonoBehaviour
{
    public bool isInQueue = false;//玩家是否在队内
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
                FindObjectOfType<QueueController>().GetComponent<QueueController>().AddToQueue(this.transform);//在队内则加入list
                flag = false;
            }      
        }
        else
        {
            if(!flag)
            {
                FindObjectOfType<QueueController>().GetComponent<QueueController>().RemoveFromQueue(this.transform);//不在则从list移除
                flag = true;
            }
            
        }
    }
}
