using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class spawnGraph : MonoBehaviour
{
    public GameObject graph;

    private Transform target;
    public Camera Camera2Follow; 
    
    public float CameraDistance = 3.0F;
    public float smoothTime = 0.3F;
    public float downwardOffset = 1.0F; // New field for downward offset

    private Vector3 velocity = Vector3.zero;


    void Start() {
        graph.SetActive(false);
    }
    void Awake()
    {
        // Position the canvas in front of the camera
        target = Camera2Follow.transform;

        // Position the graph in front of the camera with a downward offset
        target = Camera2Follow.transform;
        Vector3 targetPosition = target.TransformPoint(new Vector3(0, -downwardOffset, CameraDistance));
        transform.position = targetPosition;

        // Make the graph face the same direction as the camera
        transform.rotation = Camera2Follow.transform.rotation;
        transform.rotation *= Quaternion.Euler(0, -90, 0);
            
   
    }


    public void GraphOn() {
        graph.SetActive(true);

    }

    public void GraphOff() {
        graph.SetActive(false);
    }



}


    
