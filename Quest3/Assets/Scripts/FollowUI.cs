using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    public Camera Camera2Follow;
    public float CameraDistance = 3.0F;
    public float smoothTime = 0.3F;
    public float downwardOffset = 1.0F; // New field for downward offset

    private Vector3 velocity = Vector3.zero;
    private Transform target;

    void Awake()
    {
        target = Camera2Follow.transform;
    }

    void Update()
    {
        // Define my target position in front of the camera with a downward offset
        Vector3 targetPosition = target.TransformPoint(new Vector3(0, -downwardOffset, CameraDistance));

        // Smoothly move my object towards that position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Version 1: my object's rotation is always facing the camera with no dampening
        transform.LookAt(transform.position + Camera2Follow.transform.rotation * Vector3.forward, Camera2Follow.transform.rotation * Vector3.up);

        // Version 2: my object's rotation isn't finished synchronously with the position smooth damp
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, 35 * Time.deltaTime * smoothTime);
    }
}
