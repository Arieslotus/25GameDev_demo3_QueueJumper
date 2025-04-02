using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//����QUEUE�ϣ����ڹ��������������

public class QueueController : MonoBehaviour
{
    public List<Transform> queue_list = new List<Transform>();//���ж��������

    int lastQueuerId = -1;//���������һ���Ŷ��ߵ���ţ���������ң�

    public bool flag_firstQueuerLeave = false;//�õ�һ����������ӣ�change in inspector by hand

    int grabbedJumperId = -1;//��ǰץ����jumper�����
    public bool isPaused = false; //�����Ƿ���ͣ�ƶ�

    public Transform queuerLeaveSpot1_trans;//�Ŷ������궫���뿪��Ŀ��λ��
    public Transform jumperLeaveSpot1_trans;//����߱�ק�ߺ���ӵ�Ŀ��λ��

    void Awake()
    {
        //��ȡ���������嵽�ŵ�����
        GetChildrenRootTransforms();
    }

    void Update()
    {
        //�������
        ManageQueue();

        //������ͣ �� ����ָ� �Ŀ���
        if(grabbedJumperId != -1)
        {      
            if (isPaused)
            {           
                queue_list[grabbedJumperId + 1].GetComponent<Queuer>().enabled = false;
                queue_list[grabbedJumperId + 1].GetComponent<NavMeshAgent>().enabled = false;                
            }
            else
            {
                //Debug.LogError("eee0");

                //���Ż�����Ҫÿ֡���ã�
                //queue_list[grabbedJumperId].GetComponent<Queuer>().enabled = true;
                //queue_list[grabbedJumperId].GetComponent<NavMeshAgent>().enabled = true;
                queue_list[grabbedJumperId + 1].GetComponent<Queuer>().enabled = true;
                queue_list[grabbedJumperId + 1].GetComponent<NavMeshAgent>().enabled = true;
            }
        }


        //��һ�����������
        if (flag_firstQueuerLeave)
        {
            flag_firstQueuerLeave = false;
            Buy();
        }
    }

    void Buy()
    {
        int id = 1;
        var queuer = queue_list[id].GetComponent<Queuer>();
        if (queuer != null)
        {
            queuer.state = QueuerState.isLeaving;
            queuer.target = queuerLeaveSpot1_trans;//�Ż���ͬλ��
            queuer.behind = null;
        }
        queue_list.Remove(queue_list[id]);

        //queuer��Ϊ��queuer�ű������
    }

    void ManageQueue()
    {
        isPaused = false;

        //����ÿ�����������
        for (int i = 0; i < queue_list.Count; i++)
        {
            //����˳��������
            RenameObject(queue_list[i] , i);

            //���� ����head���������һ������������һ����Ŷ��ߣ� ����
            if(i > 0 && i < queue_list.Count -1)
            {
                //queuer-------------------------------------------------------
                var queuer = queue_list[i].GetComponent<Queuer>();
                //Debug.Log($"{queuer.name} + {queuer.enabled}");
                
                if ( queuer != null)
                {
                    if (queuer.enabled)
                    {
                        //queuer
                        queuer.target = queue_list[i - 1].transform;
                        queuer.behind = queue_list[i + 1].transform;
                        queuer.isLast = false;
                    }
                    else
                    {
                        Debug.Log($"{queuer.name} + not enable");
                    }
                }
                else
                {
                    Debug.LogError($"{queuer.name} : no queuer fond in list in ququw contrillwe");
                }

                //jumper-------------------------------------------------------

                var jumper = queue_list[i].GetComponent<Jumper>();
                if ( jumper != null && jumper.enabled)
                {
                    //�������jumper�Ǳ�ץ�������״̬��������ͣ
                    if(jumper.state == JumperState.isBeingGrabbed || jumper.state == JumperState.isJumpingInAgain)
                    {
                        isPaused = true;
                        grabbedJumperId = i;

                        //Debug.LogError("pause");
                    }
                    //�������jumper��ץ�����Զ��ӣ�jumper�Ƴ�����
                    else if(jumper.state == JumperState.isJumpingOut)
                    {         
                        RemoveFromQueue(queue_list[i]);
                        //isPaused = false;

                        grabbedJumperId = i - 1;//bug

                        //Debug.LogError("jump out");
                    }
                    else
                    {
                        //grabbedJumperId = -1;
                    }

                    //ͳһ���ú�ÿ��jumper�����ץ�����Զ��� �� Ŀ��λ��
                    if(jumper.jumperLeaveTargetPos == null)
                    {
                        jumper.jumperLeaveTargetPos = jumperLeaveSpot1_trans;//�Ż���ͬλ��
                    }
                }


            }

            //�������һ������������һ����Ŷ��ߣ���
            if (i == queue_list.Count - 1)
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
        string formattedNumber = number.ToString("D3"); // ת��Ϊ��λ����ʽ

        // ����������һ���ַ��Ƿ�Ϊ����
        if (char.IsDigit(currentName.Last()))
        {
            // �滻��������Ϊ�µ���λ��
            int lastDigitIndex = currentName.Length - 1;
            while (lastDigitIndex > 0 && char.IsDigit(currentName[lastDigitIndex - 1]))
            {
                lastDigitIndex--;
            }
            obj.name = currentName.Substring(0, lastDigitIndex) + formattedNumber;
        }
        else
        {
            // �����ƺ������λ��
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
