using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Instruction: MonoBehaviour
{
    public GameObject cube;
    public PlayableDirector timeline;
    // public GameObject L3cube;
    private bool isSpawned = false;
    // // UI
    // public GameObject generateCubeInstruction;
    // public GameObject menuInstruction;

    // public GameObject CubeText; 
    // public GameObject menuExplain;

    private Transform grabFunction;

    // Start is called before the first frame update
    void Start()
    {
        cube.SetActive(false);
        // menuInstruction.SetActive(false);
        // menuExplain.SetActive(false);

        Debug.Log("----------------" +gameObject.name + "----------------");
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
            // generateCubeInstruction.SetActive(false);
            // menuInstruction.SetActive(true);

            if (timeline != null)
            {
                timeline.Play();
            }
            else
            {
                Debug.LogError("Timeline not assigned in Instruction script.");
            }
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
        //Transform grabFunction = null;
        if (cube.activeInHierarchy) {
            grabFunction = cube.transform.Find("[BuildingBlock] HandGrab");
        } 
        Debug.Log("=======================================fix: " + grabFunction.name);

        
        if (grabFunction != null) {
            grabFunction.gameObject.SetActive(false);
            cube.GetComponent<Renderer>().enabled = false;
            // L3cube.GetComponent<Renderer>().enabled = false;

            //grabFunction.gameObject.GetComponent<Renderer>().enabled = false;
            // CubeText.SetActive(false);
            // menuExplain.SetActive(false);
            
        }
    }

    public void MovePosition() {
        Debug.Log("clicked2");
        Transform grabFunction = null;
        if (cube.activeInHierarchy) {
            grabFunction = cube.transform.Find("[BuildingBlock] HandGrab");
        } 

        Debug.Log("--------------------------------move: " + grabFunction.name);

        if (grabFunction != null) {
            grabFunction.gameObject.SetActive(true);

            cube.GetComponent<Renderer>().enabled = true;
            // CubeText.SetActive(true);
        }
    }

  

    
}
