using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityDebug = UnityEngine.Debug;

public class SpinalLogBluetoothManager : MonoBehaviour
{
    private string message;
    public float[] numbers = new float[8];
    public float forceSum;
    public BluetoothHelper BTHelper;

    public GameObject BoneGroupL3;

    public GameObject BoneGroup;


    // Start is called before the first frame update
    void Start()
    {
        try
        {
            //Debug.Log("2");
             BTHelper = BluetoothHelper.GetInstance("ESP32-SpinalLog-Kiichiro"); //device 
            BTHelper.setDeviceName("ESP32-SpinalLog-Kiichiro");
            BTHelper.OnConnected += OnConnected;
            BTHelper.setTerminatorBasedStream("\n");

            
            if (BTHelper.isDevicePaired())
            {
                BTHelper.Connect();
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
            if (BTHelper.Available)
            {
                message = BTHelper.Read(); //receive message from esp32
                numbers = ToFloatArray(message);
                forceSum = ForceSum(numbers);
                //UnityDebug.Log(message);
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
        BTHelper = BluetoothHelper.GetInstance("ESP32-SpinalLog-Kiichiro"); //device 
        BTHelper.setDeviceName("ESP32-SpinalLog-Kiichiro");
        BTHelper.OnConnected += OnConnected;
        BTHelper.setTerminatorBasedStream("\n");
        UnityDebug.Log("                                                " + BTHelper.getDeviceName());
        if (BTHelper.isConnected()) {
            BTHelper.Disconnect();
            UnityDebug.Log("DisConnected.");
            if (BTHelper.isDevicePaired() && BTHelper.getDeviceName() == "ESP32-SpinalLog-Kiichiro") {
                BTHelper.Connect(); // tries to connect
                UnityDebug.Log("Connected!!!");
                BoneGroup.SetActive(true);
                BoneGroupL3.SetActive(false);
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

    float ForceSum(float[] input) {
        float sum = 0;
        for (int i = 0; i < input.Length; i++) {
            sum += input[i];
        }
        return sum;
    }
}
