using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ShowMenu : MonoBehaviour
{

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
        if (/*leftHand.IsTracked &&*/ DetectHandFlip(leftHand)){
            menuButton.SetActive(true);
        }
        else{
            menuButton.SetActive(false);
        }
       
    }

    public void MenuControl(){
        if (!isShown){
            menu.SetActive(true);
            isShown = true;

        }
        else{
            menu.SetActive(false);
            isShown = false;
        }
    }

    bool DetectHandFlip(OVRHand hand)
    {
        // 获取手的旋转四元数
        Quaternion handRotation = hand.transform.rotation;
        // 将四元数转换为欧拉角（旋转角度）
        Vector3 handEulerAngles = handRotation.eulerAngles;
        // 通过手的局部y轴角度来判断翻转
        //Debug.Log(handEulerAngles);
        if (handEulerAngles.x > 180 && handEulerAngles.x < 358)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
