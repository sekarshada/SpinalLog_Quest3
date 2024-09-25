using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowUI : MonoBehaviour
{
    public Transform target; // The GameObject to follow
    public Camera vrCamera; // The camera to face

    void Update()
    {
        // Make the text follow the target
        if (target != null)
        {
            transform.position = target.position + new Vector3(0, 1, 0); // Adjust height as needed
        }

        // Make the text face the VR camera
        if (vrCamera != null)
        {
            transform.LookAt(vrCamera.transform);
            transform.Rotate(0, 180, 0); // Optional: flip the text so it reads correctly
        }
    }
}
