using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityDebug = UnityEngine.Debug;

public class MyBlueToothManager : MonoBehaviour
{
    string message;
    private BluetoothHelper BTHelper;

    public BoneGroupController boneGroupController;

    private bool firstConnect = true;
    public float[] numbers = new float[8];
    private float[] firstDistance = new float[8];

    public GameObject boneL2;
    public GameObject boneL3;
    public GameObject boneL4;
    public GameObject boneL5;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            //Debug.Log("2");
            BTHelper = BluetoothHelper.GetInstance("ESP32-SpinalLog-Wenyuan"); //device name
            BTHelper.OnConnected += OnConnected;
            BTHelper.setTerminatorBasedStream("\n");

            /*
            if (BTHelper.isDevicePaired())
            {
                BTHelper.Connect();
                Debug.Log("Connected!!!");
            }*/
         
          
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
                //UnityDebug.Log(message);
                if (firstConnect)
                {
                    firstDistance = toFloatArray(message);
                    firstConnect = false;
                }
                else
                {
                    numbers = toFloatArray(message);
                }
                UnityDebug.Log(message);
                


                /*upDownMove(boneL2, numbers[0], numbers[1], firstDistance[0], firstDistance[1]);
                upDownMove(boneL3, numbers[2], numbers[3], firstDistance[2], firstDistance[3]);
                upDownMove(boneL4, numbers[4], numbers[5], firstDistance[4], firstDistance[5]);
                upDownMove(boneL5, numbers[6], numbers[7], firstDistance[6], firstDistance[7]);

                selfRotation(boneL2, numbers[0], numbers[1]);
                selfRotation(boneL3, numbers[2], numbers[3]);
                selfRotation(boneL4, numbers[4], numbers[5]);
                selfRotation(boneL5, numbers[6], numbers[7]);*/
                

            }
        }
    }

    


    void OnDestroy()
    {
        if (BTHelper != null)
            BTHelper.Disconnect();
    }

    void connectBT() {
        if (!BTHelper.isConnected()) {
            if (BTHelper.isDevicePaired()) {
                BTHelper.Connect(); // tries to connect
                UnityDebug.Log("Connected!!!");
            }
        }       
    }

    public void disconnectBT() {
        if (BTHelper.isConnected()) {
            BTHelper.Disconnect();
            UnityDebug.Log("DisConnected.");
        }       
    }

    float[] toFloatArray(String message) {
        String[] input = message.Split(',');

        float[] output = new float[input.Length];

        for (int i = 0; i < input.Length; i++) {
            output[i] = float.Parse(input[i]);
        }
        return output;
    }
}
