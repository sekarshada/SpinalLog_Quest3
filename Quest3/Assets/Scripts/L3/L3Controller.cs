using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;
using UnityEngine.Analytics;

public class L3Controller : MonoBehaviour
{
    public GameObject L3;
    //private GameObject bone;
    private float leftChangeDepth;
    private float rightChangeDepth;
    public float averageChangeDepth;

    public Material objectMaterial;
    public Material whiteMaterial;
    public Material redMaterial;
    private float DEPTH_THRESHOLD = 1.0f; // Depth at which color change starts
    private float MAX_DEPTH = 10.0f; // The maximum depth for full color change

    [SerializeField]
    private L3BlueToothManager BTManager;

    // Start is called before the first frame update
    void Start()
    {
        objectMaterial.color = whiteMaterial.color;
        L3.transform.localPosition = new Vector3(0f, 0.147f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(BTManager.BTHelper != null){
            // update depth of each sensor, have to store initial distance with no pressure
            if (BTManager.BTHelper.Available)
            {
                SetCurDepth(BTManager.numbers);
                // up-down movvement, rotation
                


                // colour change
                if (averageChangeDepth > DEPTH_THRESHOLD) {
                    // Calculate the interpolation factor based on how close averageDepth is to 0
                    float t = Mathf.InverseLerp(DEPTH_THRESHOLD, MAX_DEPTH, averageChangeDepth);

                    // Interpolate between the white and red colors based on the depth
                    Color newColor = Color.Lerp(whiteMaterial.color, redMaterial.color, t);
                    //UnityDebug.Log(boneID + " color: " + newColor);

                    // Apply the interpolated color to the object's renderer
                    objectMaterial.color = newColor;
                } else
                {
                    objectMaterial.color = whiteMaterial.color;
                }
            }

            //UnityDebug.Log("average: "+ averageChangeDepth);
            if (averageChangeDepth > 0) {
                UpDownMove();
                TransverseRotation();
            }
        }
        
    }


    public void SetCurDepth(float[] input) {
        this.leftChangeDepth = input[0];
        this.rightChangeDepth = input[1];
        this.averageChangeDepth = (leftChangeDepth + rightChangeDepth) / 2;
    }

    void UpDownMove() {
        float moveDist = 0;
        //int maxDistance = 35;

        if (averageChangeDepth > 0)
        {
            
            moveDist = averageChangeDepth;

            Vector3 originalPosition = L3.transform.localPosition;

        
            L3.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, -moveDist * 0.005f);
        }       
    }

    void TransverseRotation() { //transverse rotation
        float halfDistance = Math.Abs(leftChangeDepth - rightChangeDepth)/2;
        float rotateAngle = 0;
        int boneLength = 50;
        
        if (averageChangeDepth > 0) {
            if (leftChangeDepth == 0 || rightChangeDepth == 0)
            {
                rotateAngle = Mathf.Sin(leftChangeDepth / boneLength);
            } else
            {
                rotateAngle = Mathf.Sin(halfDistance / boneLength);
            } 

            if (leftChangeDepth > rightChangeDepth)
            {
                //bone.transform.localRotation = Quaternion.Euler(0f, rotateAngle *500f, 0f);
                //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + rotateAngle);

                L3.transform.localRotation = Quaternion.Euler(0f, rotateAngle*500f, 0f);
            } else
            {
                //bone.transform.localRotation = Quaternion.Euler(0f, -rotateAngle *500f, 0f);
                //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + -rotateAngle);
                L3.transform.localRotation = Quaternion.Euler(0f, -rotateAngle*500f, 0f);
            }
            //UnityDebug.Log("rotate++++++++++++++++++++++++++++");
        }
    }
}
