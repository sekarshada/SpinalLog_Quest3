using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustPivot : MonoBehaviour
{
    public GameObject objectToAdjust;
    
    void Start()
    {
        // Calculate the current pivot offset relative to the parent
        Vector3 pivotOffset = objectToAdjust.transform.localPosition;

        // Move the objectToAdjust to the origin (0,0,0) relative to its parent
        objectToAdjust.transform.localPosition = Vector3.zero;

        // Move all children to compensate for the pivot offset
        foreach (Transform child in objectToAdjust.transform)
        {
            child.localPosition -= pivotOffset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
