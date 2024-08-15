using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    [SerializeField]
    private MyBlueToothManager BTManager;
    float a;
    // Start is called before the first frame update
    void Start()
    {
        a = BTManager.numbers[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
