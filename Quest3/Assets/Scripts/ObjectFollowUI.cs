using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowUI : MonoBehaviour
{
    public Transform target; // The GameObject to follow
    public Vector3 offset; // Offset position from the target

    void Update()
    {
        if (target != null)
        {
            // Update position with offset
            transform.position = target.position + offset;

            // Optional: Make the UI face the camera
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); // Optional, to flip the UI
        }
    }
}
