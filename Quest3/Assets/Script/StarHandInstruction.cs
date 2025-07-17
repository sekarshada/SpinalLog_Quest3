using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.Playables;

public class StarHandInstruction : MonoBehaviour
{

    public GameObject handInstructionGuide;
    public PlayableDirector director;
    // Start is called before the first frame update
   public void OnStartButtonClicked()
    {
        if (handInstructionGuide != null)
        {
            handInstructionGuide.SetActive(false);
            Debug.Log("Start Hand activated.");
        }
        else
        {
            Debug.LogWarning("Start Hand GameObject is not assigned.");
        }

        if (director != null)
        {
            director.Play();
            Debug.Log("PlayableDirector started.");
        }
        else
        {
            Debug.LogWarning("PlayableDirector is not assigned.");
        }
    }
}
