using UnityEngine;
public class HandFlip : MonoBehaviour
{
    public OVRHand leftHand;
    void Update()
    {
        DetectHandFlip(leftHand);
    }
    void DetectHandFlip(OVRHand hand)
    {
        // 获取手的旋转四元数
        Quaternion handRotation = hand.transform.rotation;
        // 将四元数转换为欧拉角（旋转角度）
        Vector3 handEulerAngles = handRotation.eulerAngles;
        // 通过手的局部y轴角度来判断翻转
        Debug.Log(handEulerAngles);
        if (handEulerAngles.x > 180 && handEulerAngles.x < 360)
        {
            Debug.Log("up");
        }
        else
        {
            Debug.Log("*******down");
        }
    }
}