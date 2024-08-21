using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

public class BoneGroupController : MonoBehaviour
{
    public GameObject boneL2;
    public GameObject boneL3;
    public GameObject boneL4;
    public GameObject boneL5;

    [SerializeField]
    private MyBlueToothManager BTManager;
    private bool firstConnect = true;

    private GameObject[] boneGroup;
    private GameObject focusBone;
    // Start is called before the first frame update
    void Start()
    {
        boneGroup = new GameObject[] {boneL2, boneL3, boneL4, boneL5};
    }

    // Update is called once per frame
    void Update()
    {
        // update depth of each sensor, have to store initial distance with no pressure
        if (firstConnect)
        {
            SetInitialBoneDepth(BTManager.numbers);
            firstConnect = false;
        }
        else
        {
            SetCurBoneDepth(BTManager.numbers);
        }
        // count rotation degree
    

    }

    public void SetInitialBoneDepth(float[] depths) {
        for (int i = 0; i < boneGroup.Length; i++) {
            boneGroup[i].GetComponent<BoneController>().setInitialDepth(depths[i*2], depths[i*2+1]);
        }
    }

    public void SetCurBoneDepth(float[] depths) {
        for (int i = 0; i < boneGroup.Length; i++) {
            boneGroup[i].GetComponent<BoneController>().setCurDepth(depths[i*2], depths[i*2+1]);
        }
    }

    float FindFocusBoneDepth() {
        float smallestDepth = 0;
        GameObject focusBone;
        for (int i = 0; i < boneGroup.Length; i++) {
            float depth = boneGroup[i].GetComponent<BoneController>().averageDepth;
            if (smallestDepth == 0 || smallestDepth > depth) {
                smallestDepth = depth;
                focusBone = boneGroup[i];
            }
        }
        return smallestDepth;
    }
}
