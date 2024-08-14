using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneTrace : MonoBehaviour
{
    public Transform target;  // Target to follow (assign your bone here)
    public float distance;  // Distance from the target

    void LateUpdate()
    {
        if (target != null)
        {
            // Update camera position to be in front of the target
            transform.position = target.position + target.forward * -distance;

            // Ensure the camera always looks at the target
            transform.LookAt(target);
        }
    }
}
