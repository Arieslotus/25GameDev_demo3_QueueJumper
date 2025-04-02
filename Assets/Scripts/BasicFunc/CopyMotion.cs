using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������ģ�ʹ�����ģ���ϸ��ƶ���
public class CopyMotion : MonoBehaviour
{
    public Transform targetLimb;//follow limb
    ConfigurableJoint cj;
    Quaternion startRot;
    public bool inverse = false;


    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        startRot = transform.localRotation;
    }

    void Update()
    {
        if (!inverse) cj.targetRotation = targetLimb.localRotation * startRot;
        else cj.targetRotation = Quaternion.Inverse(targetLimb.localRotation) * startRot;
    }
}
