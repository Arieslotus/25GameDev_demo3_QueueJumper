using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class QueueController : MonoBehaviour
{
    public List<Transform> queue_list = new List<Transform>();

    int lastQueuerId = -1;

    public bool flag = false;

    void Awake()
    {
        GetChildrenRootTransforms();
    }

    void Update()
    {
        SetFollowTarget();

        if (flag)
        {
            flag = false;
            Buy();
        }
    }

    void Buy()
    {
        queue_list.Remove(queue_list[1]);

        //queuer go
    }
    void SetFollowTarget()
    {
        for (int i = 0; i < queue_list.Count; i++)
        {
            RenameObject(queue_list[i] , i);

            if(i > 0 && i < queue_list.Count -1)
            {
                var queuer = queue_list[i].GetComponent<Queuer>();
                
                if ( queuer != null)
                {
                    //queuer
                    queuer.target = queue_list[i-1].transform;
                    queuer.behind = queue_list[i + 1].transform;
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
                    queuer.behind = null;
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

    public void RenameObject( Transform obj , int number)
    {
        string currentName = obj.name;
        string formattedNumber = number.ToString("D3"); // 转换为三位数格式

        // 检测名称最后一个字符是否为数字
        if (char.IsDigit(currentName.Last()))
        {
            // 替换最后的数字为新的三位数
            int lastDigitIndex = currentName.Length - 1;
            while (lastDigitIndex > 0 && char.IsDigit(currentName[lastDigitIndex - 1]))
            {
                lastDigitIndex--;
            }
            obj.name = currentName.Substring(0, lastDigitIndex) + formattedNumber;
        }
        else
        {
            // 在名称后添加三位数
            obj.name = currentName + formattedNumber;
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

    public void InsertIntoQueue(Transform transform , int i)
    {
        if (!queue_list.Contains(transform))
        {
            if( 0 <= i &&  i < queue_list.Count)
            {
                queue_list.Insert(i, transform);
            }
            else
            {
                Debug.LogError("worong insert in queue controller");
            }
        }
        
    }

    public void RemoveFromQueue(Transform transform) {  queue_list.Remove(transform); }
}
