using UnityEngine;
public class LockSurface : MonoBehaviour
{
    void Update()
    {
        // Keep the forward vector parallel to the XZ plane (horizontal plane)
        Vector3 forward = transform.forward;
        forward.y = 0; // Set the y-component to 0 to keep it parallel to the floor
        forward.Normalize();
        // Adjust the rotation of the cube
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}
