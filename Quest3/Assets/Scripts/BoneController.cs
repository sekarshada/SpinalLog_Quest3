using System;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

public class BoneController : MonoBehaviour
{
    public int boneID;
    //private GameObject bone;
    private float leftDepth;
    private float rightDepth;
    public float averageDepth;
    private float initialLeftDepth;
    private float initialRightDepth;

    public Renderer objectRenderer;
    public Renderer whiteRenderer;
    public Renderer redRenderer;
    public float depthThreshold = 10.0f; // Depth at which color change starts
    private float maxDepth = 1.0f; // The maximum depth for full color change

    void Start() {
        this.boneID = int.Parse(gameObject.name.Substring(1));
    }

    void Update() {
        //UnityDebug.Log(gameObject.name + " left: " + leftDepth + ", initial left: " + initialLeftDepth);
        //UnityDebug.Log(gameObject.name + " initial left: " + initialLeftDepth);
        if (initialLeftDepth != 0 && averageDepth != 0) {
            UpDownMove();
        }

        /*if (averageDepth < depthThreshold) {
            // Calculate the interpolation factor based on how close averageDepth is to 0
            float t = Mathf.InverseLerp(depthThreshold, 0, averageDepth);

            // Interpolate between the white and red colors based on the depth
            Color newColor = Color.Lerp(whiteRenderer.material.color, redRenderer.material.color, t);

            // Apply the interpolated color to the object's renderer
            objectRenderer.material.color = newColor;
        } else
        {
            // If depth is greater than threshold, revert to the whiteRenderer color
            objectRenderer.material.color = whiteRenderer.material.color;
        }*/
        /*
        if (averageDepth < depthThreshold)
        {
            // Calculate the interpolation factor based on how close averageDepth is to 0
            float t = Mathf.InverseLerp(depthThreshold, 0, averageDepth);

            // Set the target color based on the depth (gradually move toward red)
            targetColor = Color.Lerp(whiteRenderer.material.color, redRenderer.material.color, t);
        }
        else
        {
            // If depth is greater than threshold, target color should be the original white
            targetColor = whiteRenderer.material.color;
        }

        // Gradually transition to the target color
        objectRenderer.material.color = Color.Lerp(objectRenderer.material.color, targetColor, Time.deltaTime * colorChangeSpeed);
    }
        */
        
    }

    public void SetInitialDepth(float leftInput, float rightInput) {
        this.initialLeftDepth = leftInput;
        this.initialRightDepth = rightInput;
    }

    public void SetCurDepth(float leftInput, float rightInput) {
        this.leftDepth = leftInput;
        this.rightDepth = rightInput;
        this.averageDepth = (leftDepth + rightDepth) / 2;
    }

    void UpDownMove() {
        float moveDist = 0;
        //int maxDistance = 35;

        if (initialLeftDepth - leftDepth <= 0.02)
        {
            moveDist = 0;
        }
        else
        {
            moveDist = initialLeftDepth - averageDepth;

            Vector3 originalPosition = transform.localPosition;
        
            transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, -moveDist * 0.01f);
        }       
    }

    public float SelfRotationDegree() { //transverse rotation
        float halfDistance = Math.Abs(leftDepth - rightDepth)/2;
        float rotateAngle = 0;
        int boneLength = 50;
        
        if (initialLeftDepth - leftDepth <= 0.02)
        {
            return rotateAngle;
        } else {
            if (leftDepth == 0 || rightDepth == 0)
            {
                rotateAngle = Mathf.Sin(leftDepth / boneLength);
            } else
            {
                rotateAngle = Mathf.Sin(halfDistance / boneLength);
            } 

            if (leftDepth > rightDepth)
            {
                //bone.transform.localRotation = Quaternion.Euler(0f, rotateAngle *500f, 0f);
                //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + rotateAngle);
                return rotateAngle;
            } else
            {
                //bone.transform.localRotation = Quaternion.Euler(0f, -rotateAngle *500f, 0f);
                //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + -rotateAngle);
                return -rotateAngle;
            }
        }
    }

    public float GroupRotationDegree(float focusBoneDepth, int focusBoneID) {
        float rotateAngle = 0;
        float boneGap = 40; // change here
        if (initialLeftDepth - leftDepth <= 0.02)
        {
            return rotateAngle;
        } else {
            if (averageDepth == focusBoneDepth) {
                return rotateAngle;
            } else {
                float difference = averageDepth - focusBoneDepth;
                rotateAngle = Mathf.Tan(difference/boneGap);
                if (boneID < focusBoneID) {
                    return -rotateAngle;
                } else {
                    return rotateAngle;
                }
            }
        }
        
    }

    public void Rotation(float focusBoneDepth, int focusBoneID) {
        float xDegree = GroupRotationDegree(focusBoneDepth, focusBoneID);
        float yDegree = SelfRotationDegree();
        //UnityDebug.Log("xDegree: " + xDegree);
        //UnityDebug.Log("yDegree: " + yDegree);

        transform.localRotation = Quaternion.Euler(xDegree*500f, yDegree*500f, 0f);
        //transform.Rotate(xDegree*40000f, 0, 0, Space.Self);
    }

}
