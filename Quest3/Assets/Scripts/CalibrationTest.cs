using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    public GameObject cube;
    private bool isSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        //Renderer curRenderer = cube.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawned)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                DeactivateManager();
            }
        } else
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                Spawncube();
                //DeactivateManager();
            }
        }
        
        
    }

    private void Spawncube()
    {
        if (!cube.activeInHierarchy)
        {
            Vector3 handPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            handPosition.x -= 0.1f;
            //handPosition.y -= 0.1474954f;
            //handPosition.z -= 0.104012f;
            cube.transform.position = handPosition;

            //cube = Instantiate(cubePrefab, handPosition, Quaternion.Euler(0, 180, 0));
            cube.SetActive(true);

            isSpawned = true;
        }
        else
        {
            Debug.LogError("Prefab or RightHandAnchor is not set.");
        }
    }

    private void DeactivateManager()
    {
        gameObject.SetActive(false);
    }

    public void fixPosition() {
        Debug.Log("clicked1");
        Transform grabFunction = cube.transform.Find("[BuildingBlock] HandGrab");

        if (grabFunction != null) {
            grabFunction.gameObject.SetActive(false);

            cube.GetComponent<Renderer>().enabled = false;
            
        }
    }

    public void movePosition() {
        Debug.Log("clicked2");
        Transform grabFunction = cube.transform.Find("[BuildingBlock] HandGrab");

        if (grabFunction != null) {
            grabFunction.gameObject.SetActive(true);

            cube.GetComponent<Renderer>().enabled = true;
        }
    }

    
}
