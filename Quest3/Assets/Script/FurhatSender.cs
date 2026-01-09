using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class FurhatSender : MonoBehaviour
{
    public SerialReader serialReader; // Drag the SerialReader GameObject here
    public float highThreshold = 188f; // Red
    public float lowThreshold = 190f;  //  Yellow (very low)
    public int rightMostSensorIndex = 15; // Rightmost sensor = index 15
    private float lastSpokenTime = 0f;
    public float coolDownTime = 2.5f;
    private int responseCount = 0;
    public int maxResponses = 4;
    public GameObject experimentController;
    private bool wasAboveHigh = false; // edge detection
    public float responseInterval = 2.3f;

    [SerializeField] private SpinalLogBluetoothManager BTManager;
    void Start()
    {
        Debug.Log("Start Furhat");
        StartCoroutine(TestConnection());
        // StartCoroutine(SendSayRequest("HELOO FURHAT HERE -- START"));
        // StartCoroutine(SendSGestureRequest("BigSmile"));
         
    }

    IEnumerator TestConnection()
    {
        string url = "http://localhost:54321/furhat"; // ini yang untuk physical robot
        // string url = "http://172.20.10.2:54321/furhat";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            Debug.Log("Testing connection to " + url);
            yield return req.SendWebRequest();
    #if UNITY_2020_2_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
    #else
            if (req.isNetworkError || req.isHttpError)
    #endif
            {
                Debug.LogError("Connection test failed: " + req.error + " | HTTP " + req.responseCode);
            }
            else
            {
                Debug.Log("Connection OK: " + req.downloadHandler.text);
            }
        }
    }

    void Update()
    {
        SendFurhatResponse();
        Debug.Log("Force Sum in Update: " + BTManager.forceSum);
        // SendGestureResponse();
   
    }
    public void SendFurhatResponse()
    {
        if (BTManager == null || BTManager.forceSum == 0f )
            return;
        if (responseCount >= maxResponses)
            return;

        float values = BTManager.forceSum;
        Debug.Log("Force Sum in SendFurhatResponse: " + values);


        if (Time.time - lastSpokenTime > coolDownTime)
        {
            bool aboveHigh = values <= highThreshold;

            // Fire every responseInterval seconds while above threshold
            if (aboveHigh && (Time.time - lastSpokenTime) >= responseInterval)
            {
                StartCoroutine(SendSayRequest("Ouch That pressure is too high."));
                StartCoroutine(SendSGestureRequest("ExpressDisgust"));
                responseCount++;
                lastSpokenTime = Time.time;
                Debug.Log("Sent Furhat response for high pressure. Count: " + responseCount);
            }
      
        
            // else if (values > lowThreshold)
            // {
            //     Debug.Log("No pressure felt.");
            //     StartCoroutine(SendSayRequest("I do not feel anything"));
            //     responseCount++;
            //     lastSpokenTime = Time.time;
            // }

            // Only trigger on rising edge and respect cooldown
            // if (aboveHigh && !wasAboveHigh && (Time.time - lastSpokenTime) > coolDownTime)
            // {
            //     Debug.Log("Too much pressure!");
            //     StartCoroutine(SendSayRequest("Ouch That pressure is too high."));
            //     responseCount++;
            //     lastSpokenTime = Time.time;
            // }

            // update edge state
            // wasAboveHigh = aboveHigh;
            // else if (values.All(v => v < lowThreshold))
            // {
            //     Debug.Log("No pressure felt.");
            //     StartCoroutine(SendSayRequest("I do not feel anything"));
            //     responseCount++;
            //     lastSpokenTime = Time.time;
            // }
            // else if (rightMostSensorIndex >= 0 && rightMostSensorIndex < values.Length && values[rightMostSensorIndex] > lowThreshold)
            // {
            //     Debug.Log("Right spot hit!");
            //     responseCount++;
            //     StartCoroutine(SendSayRequest("Yes thats the right spot Thank you"));
            //     lastSpokenTime = Time.time;
            // }
            // }

        }


        IEnumerator SendSayRequest(string text)
        {
            string url = "http://localhost:54321/furhat/say?text=" + UnityWebRequest.EscapeURL(text); // ini yang untuk physical robot
            // string url = "http://localhost:8080/furhat/say?text=" + UnityWebRequest.EscapeURL(text);
            // string url = "http://172.20.10.2:54321/furhat/say?text=" + UnityWebRequest.EscapeURL(text);
            string json = "{\"text\": \"" + text + "\"}";
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json); // Empty body is okay
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Sending to: " + url + " with body " + json + " " + request.result + " " + request.responseCode);

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
            string url = "http://localhost:54321/furhat/gesture?name=" + UnityWebRequest.EscapeURL(gestureName); // ini yang untuk physical robot
            // string url = "http://localhost:8080/furhat/gesture?name=" + UnityWebRequest.EscapeURL(gestureName);
            // string url = "http://172.20.10.2:54321/furhat/gesture?name=" + UnityWebRequest.EscapeURL(gestureName);
            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("Gesture failed: " + request.error);
            else
                Debug.Log("Furhat gesture response: " + request.downloadHandler.text);
        }
    }
}