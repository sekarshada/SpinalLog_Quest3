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
        if (BTManager.BTHelper.Available && firstConnect)
        {
            //UnityDebug.Log("11111");
            SetInitialBoneDepth(BTManager.numbers);
            firstConnect = false;
        }
        else
        {
            SetCurBoneDepth(BTManager.numbers);
        }
        focusBone = FindFocusBoneDepth();

        // count rotation degree
       foreach (GameObject bone in boneGroup) {

            bone.GetComponent<BoneController>().rotation(focusBone.GetComponent<BoneController>().averageDepth, focusBone.GetComponent<BoneController>().boneID);
        }

    }

    public void SetInitialBoneDepth(float[] depths) {
        for (int i = 0; i < boneGroup.Length; i++) {
            boneGroup[i].GetComponent<BoneController>().SetInitialDepth(depths[i*2], depths[i*2+1]);
        }
    }

    public void SetCurBoneDepth(float[] depths) {
        for (int i = 0; i < boneGroup.Length; i++) {
            boneGroup[i].GetComponent<BoneController>().SetCurDepth(depths[i*2], depths[i*2+1]);
        }
    }

    GameObject FindFocusBoneDepth() {
        float smallestDepth = 0;
        GameObject target = null;
        for (int i = 0; i < boneGroup.Length; i++) {
            float depth = boneGroup[i].GetComponent<BoneController>().averageDepth;
            if (smallestDepth == 0 || smallestDepth > depth) {
                smallestDepth = depth;
                target = boneGroup[i];
            }
        }

        return target;
    }
}
