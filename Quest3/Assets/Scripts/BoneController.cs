using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

public class BoneController : MonoBehaviour
{
    private int boneID;
    private GameObject bone;
    private float leftDepth;
    private float rightDepth;
    private float averageDepth;
    private float initialLeftDepth;
    private float initialRightDepth;

    void Start() {
        bone = gameObject;
    }

    void UpDownMove(GameObject bone, float leftDepth, float rightDepth, float initialLeftDepth, float initialRightDepth)
    {
        float moveDist = 0;
        //int maxDistance = 35;
        float halfDistance = Math.Abs((leftDepth - rightDepth) / 2);

        if (leftDepth == initialLeftDepth && rightDepth == initialRightDepth)
        {
            moveDist = 0;
        }
        else
        {
            if (leftDepth == rightDepth) {
                moveDist = initialLeftDepth - leftDepth;
            } 
            else if (leftDepth > rightDepth) {
                moveDist = initialLeftDepth - leftDepth + halfDistance;
            } else
            {
                moveDist = initialRightDepth - rightDepth + halfDistance;
            }
        }

        Vector3 originalPosition = bone.transform.localPosition;
        
        bone.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, moveDist * 0.02f);


    }

    void SelfRotation(GameObject bone, float leftDepth, float rightDepth)
    {
        float halfDistance = Math.Abs(leftDepth - rightDepth)/2;
        float rotateAngle = 0;
        int boneLength = 50;
        if (leftDepth == 0 || rightDepth == 0)
        {
            rotateAngle = Mathf.Sin(leftDepth / boneLength);
        } else
        {
            rotateAngle = Mathf.Sin(halfDistance / boneLength);
        } 
        float originalDegree = bone.transform.eulerAngles.y;

        if (leftDepth > rightDepth)
        {
            bone.transform.localRotation = Quaternion.Euler(0f, rotateAngle *4000f, 0f);
            UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + rotateAngle);
        } else
        {
            bone.transform.localRotation = Quaternion.Euler(0f, -rotateAngle *4000f, 0f);
            UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + -rotateAngle);
        }
    }
}
