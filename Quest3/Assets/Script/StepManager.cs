using UnityEngine;
using UnityEngine.UI;
public class StepManager : MonoBehaviour
{
    public VideoUIManager ghostHand;
    public CueManager cueManager;
    public Text instructionText;
    private int currentStep = 0;
    public void StartStep()
    {
        if (currentStep == 0)
        {
            cueManager.ShowGlowZone("L4");
            instructionText.text = "Step 1: Place palm on L4";
            ghostHand.PlayVideo();
        }
        else if (currentStep == 1)
        {
            cueManager.ShowCountdown(3f);
            instructionText.text = "Step 2: Apply pressure for 3 seconds";
            ghostHand.PlayVideo(); // or next clip
        }
        else if (currentStep == 2)
        {
            cueManager.ShowArrow(new Vector3(0,0,0), new Vector3(0,0,-0.05f));
            instructionText.text = "Step 3: Glide downward";
            ghostHand.PlayVideo();
        }
    }
    public void NextStep()
    {
        currentStep++;
        StartStep();
    }
    public void ReplayStep()
    {
        StartStep();
    }
}






