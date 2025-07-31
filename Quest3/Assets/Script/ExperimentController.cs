using UnityEngine;
public class ExperimentController : MonoBehaviour
{
    public GameObject forceGraph;
    public GameObject heatmap;
    public GameObject robotFeedback;
    public GameObject handInstruction;
    public GameObject UIInstruction;
    [Range(1, 4)]
    public int condition = 1;
    void Start()
    {
        ApplyCondition(condition);
        forceGraph.SetActive(false);
        heatmap.SetActive(false);
        robotFeedback.SetActive(false);
        handInstruction.SetActive(false);
        UIInstruction.SetActive(false);
        

    }
    public void ApplyCondition(int cond)
    {
        switch (cond)
        {
            case 1: // Graph only
                forceGraph.SetActive(true);
                heatmap.SetActive(false);
                robotFeedback.SetActive(false);
                break;
            case 2: // Graph + Heatmap
                forceGraph.SetActive(true);
                heatmap.SetActive(true);
                robotFeedback.SetActive(false);
                break;
            case 3: // Graph + Robot
                forceGraph.SetActive(true);
                heatmap.SetActive(false);
                robotFeedback.SetActive(true);
                break;
            case 4: // Graph + Robot + Heatmap
                forceGraph.SetActive(true);
                heatmap.SetActive(true);
                robotFeedback.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown condition value: " + cond);
                break;
        }
    }
}





