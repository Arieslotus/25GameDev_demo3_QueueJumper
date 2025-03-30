using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    public Transform targetLimb;//follow limb
    ConfigurableJoint cj;
    Quaternion startRot;
    public bool inverse = false;

    //void Start()
    //{
    //    cj = GetComponent<ConfigurableJoint>();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    cj.targetRotation = targetLimb.rotation;
    //    //transform.rotation = targetLimb.rotation;
    //    //cj.targetPosition = targetLimb.position;
    //}

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
