using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class spawnVideo : MonoBehaviour
{
    public GameObject video;

    private Transform target;
    public Camera Camera2Follow; 
    
    public float CameraDistance = 0.5F;
    public float smoothTime = 0.3F;
    public float downwardOffset = 2.0F; // New field for downward offset

    private Vector3 velocity = Vector3.zero;


    void Start() {
        Debug.Log(video.activeInHierarchy);
        video.SetActive(false);
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
        transform.rotation *= Quaternion.Euler(0, 0, 0);
        var lp = transform.localPosition;
        lp.y = -0.5f;
        transform.localPosition = lp;
   
    }


    public void VideoOn() {
       if (video == null) {
            Debug.LogWarning("VideoOn called but 'video' is null. Assign it in the Inspector.", this);
            return;
        }
        video.SetActive(true);
        Debug.Log($"Video On -> {video.name} activeSelf={video.activeSelf} activeInHierarchy={video.activeInHierarchy}", this);
    }

    public void VideoOff() {
        video.SetActive(false);
    }



}


    
