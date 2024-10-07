using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnHide : MonoBehaviour
{
    public GameObject showHideGraph;
    public GameObject changeStiffness;
    public GameObject connectionSpinalLog;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (connectionSpinalLog.activeInHierarchy){
            showHideGraph.SetActive(false);
            changeStiffness.SetActive(false);
        }
        else{
            showHideGraph.SetActive(true);
            changeStiffness.SetActive(true);
        }
        
    }
}
