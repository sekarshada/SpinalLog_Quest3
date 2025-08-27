using Mono.Reflection;
using UnityEngine;
public class ExperimentController : MonoBehaviour
{
    public GameObject forceGraph;
    public GameObject heatmap;
    public GameObject robotFeedback;
    // public GameObject handInstruction;
    public GameObject CanvasVideo;
    public Instruction instruction;

    public VideoUIManager videoUIManager;
    [Range(1, 4)]
    public int condition = 1;
    void Start()
    {   

        ApplyCondition(condition);
        Debug.Log("ExperimentController started with condition: " + condition);
        
    }
    public void ApplyCondition(int cond)
    {
        switch (cond)
        {
            case 1: // Graph only
                robotFeedback.SetActive(false);
                break;
            case 2: // Graph + Heatmap
                robotFeedback.SetActive(false);
                instruction.Spawncube();
                break;
            case 3: // Graph + Robot
                robotFeedback.SetActive(true);
                break;
            case 4: // Graph + Robot + Heatmap              
                robotFeedback.SetActive(true);
                instruction.Spawncube();
                break;
            default:
                Debug.LogWarning("Unknown condition value: " + cond);
                break;
        }
    }
}





