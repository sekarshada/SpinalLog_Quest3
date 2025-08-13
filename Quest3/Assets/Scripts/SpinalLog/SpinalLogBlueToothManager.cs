using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityDebug = UnityEngine.Debug;
using XCharts.Runtime;
using System.Text.RegularExpressions;
public class SpinalLogBluetoothManager : MonoBehaviour
{
    private string message;
    public float[] numbers = new float[8];
    public float forceSum;

    public int[] forceMatrix;
    public float[] distances;
    public BluetoothHelper BTHelper;

    public GameObject BoneGroup;

    public GameObject L3Cube;

    public GameObject spinalLogCube;

    public GameObject currentstatusLocked;
    Calibration calibration;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            UnityDebug.Log("SpinalLogBluetoothManager Start");
            BTHelper = BluetoothHelper.GetInstance("ESP32-vARtebrae-Gabriella"); 
            // BTHelper.setDeviceName("ESP32-vARtebrae-Gabriella");
            BTHelper.OnConnected += OnConnected;
            BTHelper.setTerminatorBasedStream("\n");

            
           UnityDebug.Log("Bluetooth Start — looking for device: " + BTHelper.getDeviceName());
            if (BTHelper.isDevicePaired()) {
                UnityDebug.Log("Device is paired. Attempting to connect...");
                BTHelper.Connect();
                UnityDebug.Log("===========================spinallog connect");
            } else {
                UnityDebug.LogError("Device is NOT paired: " + BTHelper.getDeviceName());
            }
         
          
        }
        catch (BluetoothHelper.BlueToothNotEnabledException ex) { 
            Console.WriteLine("BlueTooth not enabled"); 
        }
        catch (BluetoothHelper.BlueToothNotSupportedException ex) { 
            Console.WriteLine("BlueTooth not supported"); 
        }
        catch (BluetoothHelper.BlueToothNotReadyException ex) { 
            Console.WriteLine("BlueTooth not ready"); 
        }
    }

    void OnConnected()
    {
        BTHelper.StartListening();
        BTHelper.SendData("Hi esp32! "); // this can be called anywhere
    }



    // Update is called once per frame
    void Update()
    {
        if (BTHelper != null)
        {
            // UnityDebug.Log(BTHelper.Available);
            if (BTHelper.Available)
            {

                message = BTHelper.Read(); //receive message from esp32
                 ParseMessage(message);
                // forceSum = ForceSum(numbers);

                // UnityDebug.Log("BluetoothManager attached to: " + gameObject.name + " | Received: " + message);
                //UnityDebug.Log(forceSum);
            }
        }
    }


    void OnDestroy()
    {
        if (BTHelper != null)
            BTHelper.Disconnect();
    }

    public void ConnectBT() { 
        
        // Disable the script
        //BTHelper = BluetoothHelper.GetNewInstance("ESP32-SpinalLog-Kiichiro");
        BTHelper = BluetoothHelper.GetInstance("ESP32-vARtebrae-Gabriella"); //device name
        BTHelper.setDeviceName("ESP32-vARtebrae-Gabriella");
        BTHelper.OnConnected += OnConnected;
        BTHelper.setTerminatorBasedStream("\n");
        UnityDebug.Log("connect to" + BTHelper.getDeviceName());
        if (BTHelper.isConnected()) {
            BTHelper.Disconnect();
            UnityDebug.Log("L3 DisConnected.");
            if (BTHelper.isDevicePaired() && BTHelper.getDeviceName() == "ESP32-vARtebrae-Gabriella") {
                BTHelper.Connect(); // tries to connect
                UnityDebug.Log("spinallog Connected!!!");
                spinalLogCube.SetActive(true);
                BoneGroup.SetActive(true);
                spinalLogCube.transform.position = L3Cube.transform.position;
                L3Cube.SetActive(false);

                if (currentstatusLocked.activeInHierarchy){
                    calibration.FixPosition();
                }

  
            }
        }  
       //elseif (!BTHelper.isConnected()) {
          
    }


    public void DisconnectBT() {
        if (BTHelper.isConnected()) {
            BTHelper.Disconnect();
            UnityDebug.Log("DisConnected.");
        }       
    }

    float[] ToFloatArray(String message) {
        String[] input = message.Split(',');

        float[] output = new float[input.Length];

        for (int i = 0; i < input.Length; i++) {
            output[i] = System.Single.Parse(input[i]);
        }
        return output;
    }

    void ParseMessage(string msg)
    {
        string[] parts = msg.Split(',');
        if (parts.Length != 107)
        {
            UnityDebug.LogWarning($"Invalid message length: {parts.Length} (expected 107). Message: {msg}");
            return;
        }
        forceMatrix = new int[99];
        distances = new float[8];
        for (int i = 0; i < 99; i++)
        {
            if (!int.TryParse(parts[i], out forceMatrix[i]))
            {
                UnityDebug.LogError($"Failed to parse int at index {i}: '{parts[i]}'");
                return;
            }
        }
        for (int i = 0; i < 8; i++)
        {
            if (!float.TryParse(parts[99 + i], System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out distances[i]))
            {
                UnityDebug.LogError($"Failed to parse float at index {99 + i}: '{parts[99 + i]}'");
                return;
            }
        }
        forceSum = ForceSum(distances);
        // Optional: debug logs
        // UnityDebug.Log($"Force Matrix: {string.Join(", ", forceMatrix)}");
        // UnityDebug.Log($"Distances: {string.Join(", ", distances)}");
    }
    float ForceSum(float[] input)
    {
        float sum = 0;
        for (int i = 0; i < input.Length; i++)
        {
            sum += input[i];
        }
        // UnityDebug.Log("Force Sum: " + sum);
        return sum;

       
        
    }
}
