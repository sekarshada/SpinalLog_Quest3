using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;
public class Instruction: MonoBehaviour
{
    public GameObject cube;
    public GameObject instructionGuide;
    private bool isSpawned = false;
     public VideoPlayer videoPlayer;
    private Transform grabFunction;

    public ExperimentController experimentController;

    // Start is called before the first frame update
    void Start()
    {
        cube.SetActive(false);
        Debug.Log("----------------" + gameObject.name + "----------------");

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
            if (OVRInput.GetDown(OVRInput.Button.One) && experimentController.condition != 1)
            {
                Spawncube();
                Debug.Log("Spawned cube");
                //DeactivateManager();
            }
        }
        
        
    }

    public void Spawncube()
    {
        if (!cube.activeInHierarchy)
        {
            Vector3 handPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            handPosition.x -= 0.1f;
            //handPosition.y -= 0.1474954f;
            //handPosition.z -= 0.104012f;
            cube.transform.position = handPosition;
            videoPlayer.Play();

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
