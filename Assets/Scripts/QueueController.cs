using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueueController : MonoBehaviour
{
    public List<Transform> queue_list = new List<Transform>();

    int lastQueuerId = -1;

    void Start()
    {
        GetChildrenRootTransforms();
    }

    void Update()
    {
        SetFollowTarget();
    }


    void SetFollowTarget()
    {
        for (int i = 0; i < queue_list.Count; i++)
        {
            if(i > 0 && i < queue_list.Count -1)
            {
                var queuer = queue_list[i].GetComponent<Queuer>();
                
                if ( queuer != null)
                {
                    //queuer
                    queuer.target = queue_list[i-1].transform;
                    queue_list[i].GetComponent<Queuer>().isLast = false;
                }
                else
                {
                    Debug.LogError("no queuer fond in list in ququw contrillwe");
                }

                
            }

            if(i == queue_list.Count - 1)
            {
                var queuer = queue_list[i].GetComponent<Queuer>();
                var playerQueuer = queue_list[i].GetComponent<PlayerQueuer>();
                if (playerQueuer != null && queuer == null)
                {
                    //player in queue
                    lastQueuerId = i - 1;

                }
                else if(queuer != null && playerQueuer ==null)
                {
                    //player out queue
                    queuer.target = queue_list[i - 1].transform;
                    lastQueuerId = i;
                }
                else
                {
                    Debug.LogError("no queuer fond in list in ququw contrillwe");
                }

                queue_list[lastQueuerId].GetComponent<Queuer>().isLast = true;
            }
        }

        
    }

    void GetChildrenRootTransforms()
    {
        queue_list.Clear();

        foreach (Transform child in transform)
        {
            if(child.name == "head")
            {
                //queue_list.Add(child);
                queue_list.Insert(0, child);
            }
            else
            {
                if (child.childCount > 0)
                {
                    queue_list.Add(child.GetChild(1));

                }
            }

        }
    }

    public void AddToQueue(Transform transform)
    {
        //add player in last
        queue_list.Add(transform);
    }

    public void RemoveFromQueue(Transform transform) {  queue_list.Remove(transform); }
}
