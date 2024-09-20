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

    public float changeDepth;

    public float transverseAngle;

    public float saggitalAngle;

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
                //Debug.Log(BTManager.numbers);
                SetData(BTManager.numbers);
                // up-down movvement, rotation
                


                // colour change
                if (changeDepth > DEPTH_THRESHOLD) {
                    // Calculate the interpolation factor based on how close averageDepth is to 0
                    float t = Mathf.InverseLerp(DEPTH_THRESHOLD, MAX_DEPTH, changeDepth);

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

            //Rotation();

            //UnityDebug.Log("average: "+ averageChangeDepth);
            if (changeDepth > 0) {
                UpDownMove();
                Rotation();
            }
        }
        
    }


    public void SetData(float[] input) {
        this.changeDepth = input[0];
        this.transverseAngle = input[1];
        this.saggitalAngle = input[2];
        //Debug.Log(transverseAngle);
    }

    void UpDownMove() {
        float moveDist = 0;
        //int maxDistance = 35;

        if (changeDepth > 0)
        {
            
            moveDist = changeDepth;

            Vector3 originalPosition = L3.transform.localPosition;

        
            L3.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, -moveDist * 0.005f);
        }       
    }

    void Rotation() { //transverse rotation


        L3.transform.localRotation = Quaternion.Euler(saggitalAngle, transverseAngle, 0f);
 
        //UnityDebug.Log("rotate++++++++++++++++++++++++++++");
        
    }
}
