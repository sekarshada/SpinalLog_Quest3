using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    public GameObject frankaPrefab;
    private GameObject franka;
    private bool isSpawned = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (isSpawned)
        {
            textComponent.text = "Only one Franka is allowed for now.\nPress A to exit.";
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                textComponent.text = "";
                DeactivateManager();
            }
        } else
        {}*/
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SpawnFranka();
            //DeactivateManager();
        }
        
    }

    private void SpawnFranka()
    {
        if (frankaPrefab != null)
        {
            Vector3 handPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            handPosition.x -= 0.0033f;
            handPosition.y -= 0.1474954f;
            handPosition.z -= 0.104012f;

            franka = Instantiate(frankaPrefab, handPosition, Quaternion.Euler(0, 180, 0));
            franka.SetActive(true);

            /*jointController = franka.GetComponent<JointController>();
            gripperController = franka.GetComponent<GripperController>();
            moveToStart = franka.GetComponent<MoveToStart>();
            syncFromFranka = franka.GetComponent<SyncFromFranka>();
            
            moveBase = franka.GetComponent<MoveBase>();
            reachTarget = franka.GetComponent<ReachTarget>();
            followTarget = franka.GetComponent<FollowTarget>();
            followTrajectory = franka.GetComponent<FollowTrajectory>();
            // jointsPublisher = franka.GetComponent<JointsPublisher>();

            invisibleFranka = franka.GetComponent<InvisibleFranka>();
            planeManager = franka.GetComponent<PlaneManager>();

            visJointPos = franka.GetComponent<VisualiseJointPositions>();
            visManip = franka.GetComponent<VisualiseManipulability>();
            
            isSpawned = true;
            ActivateToggles();*/
        }
        else
        {
            Debug.LogError("Prefab or RightHandAnchor is not set.");
        }
    }
}
