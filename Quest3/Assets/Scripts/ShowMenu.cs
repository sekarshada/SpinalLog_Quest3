using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ShowMenu : MonoBehaviour
{
    public GameObject menuInstruction;
    public GameObject menu;
    public GameObject menuButton;
    public OVRHand leftHand;
    
    private bool isShown = false;
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        menuButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (DetectHandFlip(leftHand)){
            menuButton.SetActive(true);
        }
        else{
            menuButton.SetActive(false);
            menu.SetActive(false);
        }
       
    }

    public void MenuControl(){
        if (!isShown){
            menu.SetActive(true);
            isShown = true;
            menuInstruction.SetActive(false);
        }
        else{
            menu.SetActive(false);
            isShown = false;
        }
    }

    bool DetectHandFlip(OVRHand hand)
    {
        // get hand rotation degree
        Quaternion handRotation = hand.transform.rotation;
        Vector3 handEulerAngles = handRotation.eulerAngles;
       
        // hand face up
        if (handEulerAngles.x > 180 && handEulerAngles.x < 359)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
