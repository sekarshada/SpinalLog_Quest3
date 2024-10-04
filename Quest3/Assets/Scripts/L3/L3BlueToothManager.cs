using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityDebug = UnityEngine.Debug;
using UnityEngine.UI;

public class L3BlueToothManager : MonoBehaviour
{
    public static L3BlueToothManager l3Manager;
    private string message;
    public float[] numbers = new float[3];

    public float forceSum;
    public BluetoothHelper BTHelper;

    public GameObject L3Cube;

    public GameObject spinalLogCube;

    private void Awake()
    {
        if (l3Manager == null)
        {
            l3Manager = this; // Assign the singleton instance
            DontDestroyOnLoad(gameObject); // Optional: Keep it alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    // Start is called before the first frame update
    void Start()
    {
     
        L3Cube.SetActive(false);
       /* try
        {
            BTHelper = BluetoothHelper.GetInstance("ESP32-10_2"); //device 
            //BTHelper.setDeviceName("ESP32-SpinalLog-Kiichiro");
            BTHelper.OnConnected += OnConnected;
            BTHelper.setTerminatorBasedStream("\n");

            
            if (BTHelper.isDevicePaired())
            {
                BTHelper.Connect();
                UnityDebug.Log("connect");
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
        }*/
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
            //.Log("connect to " + BTHelper.getDeviceName());
            if (BTHelper.Available)
            {
                message = BTHelper.Read(); //receive message from esp32
                numbers = ToFloatArray(message);
                //UnityDebug.Log("-------------------"+numbers[0]);
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
    
        //BTHelper = BluetoothHelper.GetInstance("ESP32-SpinalLog-Wenyuan"); //device name
        //BTHelper.setDeviceName("ESP32-SpinalLog-Wenyuan");
        BTHelper = BluetoothHelper.GetInstance("ESP32-10_2"); //device name
        BTHelper.setDeviceName("ESP32-10_2");
        BTHelper.OnConnected += OnConnected;
        BTHelper.setTerminatorBasedStream("\n");
        UnityDebug.Log("                                                " + BTHelper.getDeviceName());

        if (BTHelper.isConnected()) {
            BTHelper.Disconnect();
            UnityDebug.Log("spinallog DisConnected.");
            if (BTHelper.isDevicePaired() && BTHelper.getDeviceName() == "ESP32-10_2") {

                BTHelper.Connect(); // tries to connect
                UnityDebug.Log("L3 Connected!!!");
                spinalLogCube.SetActive(false);
                L3Cube.transform.position = spinalLogCube.transform.position;
                L3Cube.SetActive(true);
              
            }
        }  
        //if (!BTHelper.isConnected()) {
         
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
            output[i] = float.Parse(input[i]);
        }
        return output;
    }



    public void sendData1(){
        BTHelper.SendData("1");
    }
    public void sendData2(){
        BTHelper.SendData("2");
    }
    public void sendData3(){
        BTHelper.SendData("3");
    }

 
}
