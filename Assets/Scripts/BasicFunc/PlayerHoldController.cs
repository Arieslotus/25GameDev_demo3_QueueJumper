using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������hip�ϣ����ڿ��Ʊ�ץס��������أ�ͳһ�����������hand�����������Ϣ

public class PlayerHoldController : MonoBehaviour
{
    public Animator animator;//���ƶ�����ģ�͵�animator
    public GameObject grabbedObj;//��ǰץס�����壨�Զ����ã�
    bool flag;
    public bool flag_canHold;//��ǰ�Ƿ���ץ����������ץ���������������1�ɿ�����2jumper�����Զ�Զ�����
    [HideInInspector]public bool jumperOutDistance = false;//��ǰץ��jumper�Ƿ�����Զ��set in jumpers
    public Rigidbody handR_rb , handL_rb;//�����ֵ�rb

    void Start()
    {
        flag = true;
        flag_canHold = false;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(2))//Ŀǰ������м�����סץȡ�����Ż���
        {

            animator.SetBool("isHold", true);
            flag_canHold = true;


        }
        else if (Input.GetMouseButton(2))
        {

        }
        else if (Input.GetMouseButtonUp(2))
        {

            animator.SetBool("isHold", false);
            flag_canHold = false;

        }

        if (!flag_canHold)
        {
            LetGo();
        }
        else
        {
            if (grabbedObj != null)
            {
                StartGrab();
            }
        }
    }

    void StartGrab()
    {
        if (flag)
        {
            //���ñ�ץ��������ֵ����ӵ�
            //���Ż�ik��
            FixedJoint jointL = grabbedObj.AddComponent<FixedJoint>();
            jointL.connectedBody = handL_rb;
            jointL.breakForce = 900;//��Ҫ̫�󣬲�Ȼ����ײ�崩��鴤
            
            FixedJoint jointR = grabbedObj.AddComponent<FixedJoint>();
            jointR.connectedBody = handR_rb;
            jointR.breakForce = 900;

            //���ץ��jumper
            if (grabbedObj.tag == "jumper")
            {
                Debug.LogWarning($"grab: {grabbedObj.name}");

                //��ȡ�������е�jumper���������jumper״̬Ϊ��ץ
                Transform current = grabbedObj.transform;
                var jumper = current.GetComponent<Jumper>();
                while (current != null)
                {
                    jumper = current.GetComponent<Jumper>();
                    if (jumper != null)
                    {
                        jumper.state = JumperState.isBeingGrabbed;
                        break; // �ҵ���ֱ�ӷ���
                    }
                    current = current.parent; // �������ϱ���
                }
                if (jumper == null)
                {
                    Debug.LogError("jumper tag but no Jumper component found in any parent.");
                }
            }

            flag = false;//���ƺ���ִ��һ��

            jumperOutDistance = false;//bug
        }
        
    }

    void LetGo()
    {
        if (grabbedObj != null)
        {
            //ɾ�����ӵ㣨�������ӵ㲻ֹһ��������������ܵ������Σ�
            //Debug.LogError("delet");
            Destroy(grabbedObj.GetComponent<FixedJoint>());

            //������ӵ㶼ɾ����
            if (grabbedObj.GetComponent<FixedJoint>() == null)
            {
                //���ץ����jumper������ʱ��Ҫ�����Ƿ񳬹���Զ���룬��ȡ������jumper���������jumper״̬
                if (grabbedObj.tag == "jumper")
                {
                    Transform current = grabbedObj.transform;
                    var jumper = current.GetComponent<Jumper>();

                    while (current != null)
                    {
                        jumper = current.GetComponent<Jumper>();
                        if (jumper != null)
                        {
                            if (jumperOutDistance)
                            {
                                jumper.state = JumperState.isJumpingOut;//������������jumperΪ���
                                //Debug.LogError("hold ser jump out");
                            }
                            else
                            {
                                jumper.state = JumperState.isJumpingInAgain;//δ������������jumper�ض�
                                //Debug.LogError("hold set jump in");
                            }                     
                            break; // �ҵ���ֱ�ӷ���
                        }
                        current = current.parent; // �������ϱ���������
                    }

                    if (jumper == null)
                    {
                        Debug.LogError("jumper tag but no Jumper component found in any parent.");
                    }

                }

                //ȷ��ɾ���������ӵ㣬�����ú�jumper״̬��������ץ����Ϊ��
                grabbedObj = null;
                flag = true;
                
            }

        }
    }

}
