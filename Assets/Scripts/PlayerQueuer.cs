using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQueuer : MonoBehaviour
{
    public bool isInQueue = false;
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
                FindObjectOfType<QueueController>().GetComponent<QueueController>().AddToQueue(this.transform);
                flag = false;
            }      
        }
        else
        {
            if(!flag)
            {
                FindObjectOfType<QueueController>().GetComponent<QueueController>().RemoveFromQueue(this.transform);
                flag = true;
            }
            
        }
    }
}
