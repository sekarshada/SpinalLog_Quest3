using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

public class BoneController : MonoBehaviour
{
    //private int boneID;
    //private GameObject bone;
    private float leftDepth;
    private float rightDepth;
    public float averageDepth;
    private float initialLeftDepth;
    private float initialRightDepth;

    void Start() {}

    void Update() {
        if (initialLeftDepth != 0 && averageDepth != 0) {
            UpDownMove();
        }
        
    }

    public void setInitialDepth(float leftInput, float rightInput) {
        this.initialLeftDepth = leftInput;
        this.initialRightDepth = rightInput;
    }

    public void setCurDepth(float leftInput, float rightInput) {
        this.leftDepth = leftInput;
        this.rightDepth = rightInput;
        this.averageDepth = (leftDepth + rightDepth) / 2;
    }

    void UpDownMove() {
        float moveDist = 0;
        //int maxDistance = 35;

        if (averageDepth == initialLeftDepth)
        {
            moveDist = 0;
        }
        else
        {
            moveDist = initialLeftDepth - averageDepth;

            /*if (leftDepth == rightDepth) {
                moveDist = initialLeftDepth - leftDepth;
            } 
            else if (leftDepth > rightDepth) {
                moveDist = initialLeftDepth - leftDepth + halfDistance;
            } else
            {
                moveDist = initialRightDepth - rightDepth + halfDistance;
            }*/
        }

        Vector3 originalPosition = transform.localPosition;
        
        transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, moveDist * 0.02f);
    }

    public float SelfRotation() {
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

        if (leftDepth > rightDepth)
        {
            //bone.transform.localRotation = Quaternion.Euler(0f, rotateAngle *4000f, 0f);
            //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + rotateAngle);
            return rotateAngle;
        } else
        {
            //bone.transform.localRotation = Quaternion.Euler(0f, -rotateAngle *4000f, 0f);
            //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + -rotateAngle);
            return -rotateAngle;
        }
    }

    public float groupRotation(float focusBoneDepth) {
        float rotateAngle = 0;
        float boneGap = 40; // change here
        if (averageDepth == focusBoneDepth) {
            return rotateAngle;
        } else {
            float difference = averageDepth - focusBoneDepth;
            rotateAngle = Mathf.Tan(difference/boneGap);
            return rotateAngle;
        }
    }
}
