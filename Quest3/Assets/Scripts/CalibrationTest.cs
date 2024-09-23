using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    public GameObject cube;
    private bool isSpawned = false;
    // UI
    public GameObject generateCubeInstruction;
    public GameObject menuInstruction;
    

    // Start is called before the first frame update
    void Start()
    {
        cube.SetActive(false);
        menuInstruction.SetActive(false);
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
            generateCubeInstruction.SetActive(false);
            menuInstruction.SetActive(true);
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

    public void FixPosition() {
        Debug.Log("clicked1");
        Transform grabFunction = cube.transform.Find("[BuildingBlock] HandGrab");

        if (grabFunction != null) {
            grabFunction.gameObject.SetActive(false);

            cube.GetComponent<Renderer>().enabled = false;
            
        }
    }

    public void MovePosition() {
        Debug.Log("clicked2");
        Transform grabFunction = cube.transform.Find("[BuildingBlock] HandGrab");

        if (grabFunction != null) {
            grabFunction.gameObject.SetActive(true);

            cube.GetComponent<Renderer>().enabled = true;
        }
    }

    void GetFocusBone(float[] numbers) {
        float[] bonesDepth = new float[4];
  
        bonesDepth[0] = (numbers[0] + numbers[1])/2;
        bonesDepth[1] = (numbers[2] + numbers[3])/2;
        bonesDepth[2] = (numbers[4] + numbers[5])/2;
        bonesDepth[3] = (numbers[6] + numbers[7])/2;

        int DepthPosition = 0;
        float smallestDist = 0;
        for (int i = 0; i < bonesDepth.Length; i++) {
            if (smallestDist == 0 || bonesDepth[i] < smallestDist) {
                DepthPosition = i;
                smallestDist = bonesDepth[i];
            }
        }
        

    }

    
}
