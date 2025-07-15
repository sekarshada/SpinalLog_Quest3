using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class FurhatSender : MonoBehaviour
{
    public SerialReader serialReader; // Drag the SerialReader GameObject here
    public float highThreshold = 0.2f; // Red
    public float lowThreshold = 0.05f;  //  Yellow (very low)
    public int rightMostSensorIndex = 15; // Rightmost sensor = index 15
    private float lastSpokenTime = 0f;
    public float coolDownTime = 2.5f;


    void Start()
    {
        Debug.Log("Start Furhat");
        StartCoroutine(SendSayRequest("HELOO FURHAT HERE -- START"));
        StartCoroutine(SendSGestureRequest("BigSmile"));
    }

    void Update()
    {
        if (serialReader == null || serialReader.normalizedValues == null)
            return;
        float[] values = serialReader.normalizedValues;
        // Log values to debug
        // Debug.Log("Sensor values: " + string.Join(", ", values.Select(v => v.ToString("F2"))));
        Debug.Log(values[1] + "Values 1");
        // if (values.Any(v => v > highThreshold))

        if (Time.time - lastSpokenTime > coolDownTime)
        {
            if (values[1] > highThreshold)
            {
                Debug.Log("Too much pressure!");
                StartCoroutine(SendSayRequest("Ouch That pressure is too high."));
                StartCoroutine(SendSGestureRequest("ExpressDisgust"));
                lastSpokenTime = Time.time;
            }
            else if (values.All(v => v < lowThreshold))
            {
                Debug.Log("No pressure felt.");
                StartCoroutine(SendSayRequest("I do not feel anything"));
                lastSpokenTime = Time.time;
            }
            else if (values[rightMostSensorIndex] > lowThreshold)
            {
                Debug.Log("Right spot hit!");
                StartCoroutine(SendSayRequest("Yes thats the right spot Thank you"));
                lastSpokenTime = Time.time;
            }
        }
    }

    IEnumerator SendSayRequest(string text)
    {
        string url = "http://localhost:54321/furhat/say?text=" + UnityWebRequest.EscapeURL(text);
        string json = "{\"text\": \"" + text + "\"}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json); // Empty body is okay
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Sending to: " + url + "with body" + json);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Furhat API call failed: " + request.error);
        else
            Debug.Log("Furhat responded: " + request.downloadHandler.text);
    }

    // // IEnumerator SendSGestureRequest(string name)
    // // {
    // //     string url = "http://localhost:54321/furhat/gesture?name=" + UnityWebRequest.EscapeURL(name);
    // //     // string url = "http://localhost:54321/furhat/gesture";
    // //     //    string json = "{\"name\": \"" + name + "\", \"class\": \"furhatos.gestures.Gesture\"}";
    // //     // string json = "{\"name\": \"" + name + "\"}";
    // //     UnityWebRequest request = new UnityWebRequest(url, "POST");
    // //     byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json); // Empty body is okay
    // //     request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    // //     request.downloadHandler = new DownloadHandlerBuffer();
    // //     request.SetRequestHeader("Content-Type", "application/json");
    // //     // Debug.Log("Sending to: " + url + "with body" + json);
    // //     yield return request.SendWebRequest();
    // //     if (request.result != UnityWebRequest.Result.Success)
    // //         Debug.LogError("Furhat API call failed: " + request.error);
    // //     else
    // //         Debug.Log("Furhat responded: " + request.downloadHandler.text);
    // // }
    
    IEnumerator SendSGestureRequest(string gestureName)
    {
        string url = "http://localhost:54321/furhat/gesture?name=" + UnityWebRequest.EscapeURL(gestureName);
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Gesture failed: " + request.error);
        else
            Debug.Log("Furhat gesture response: " + request.downloadHandler.text);
    }

}