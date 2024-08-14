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
            BTHelper = BluetoothHelper.GetInstance("ESP32-SpinalLog-Kiichiro"); //device name
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

                float[] numbers = toFloatArray(message);
                UnityDebug.Log(message);

                upDownMove(boneL2, numbers[0], numbers[1]);
                upDownMove(boneL3, numbers[2], numbers[3]);
                upDownMove(boneL4, numbers[4], numbers[5]);
                upDownMove(boneL5, numbers[6], numbers[7]);

                rotationY(boneL2, numbers[0], numbers[1]);
                rotationY(boneL3, numbers[2], numbers[3]);
                rotationY(boneL4, numbers[4], numbers[5]);
                rotationY(boneL5, numbers[6], numbers[7]);


            }
        }
    }

    
    /*void OnGUI()
    {

        if (BTHelper == null)
            return;


        BTHelper.DrawGUI();

        if (!BTHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Connect"))
            {
                if (BTHelper.isDevicePaired())
                    BTHelper.Connect(); // tries to connect
                    UnityDebug.Log("Connected!!!");
            }

        if (BTHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height - 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                BTHelper.Disconnect();
                UnityDebug.Log("DisConnected.");
            }
    }*/

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

    void upDownMove(GameObject bone, float left, float right)
    {
        float moveDist = 0;
        //int maxDistance = 35;
        float halfDistance = Math.Abs((left - right) / 2);
        if (left == right) {
            moveDist = left;
        } else if (left > right) {
            moveDist = right + halfDistance;
        } else
        {
            moveDist = left + halfDistance;
        }

        Vector3 originalPosition = bone.transform.localPosition;

        bone.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, -moveDist * 0.01f);
        //UnityDebug.Log("----origin: " + originalPosition.y + ", now: " + bone.transform.position.y);
    }

    void rotationY (GameObject bone, float left, float right)
    {
        float halfDistance = Math.Abs(left - right);
        float rotateAngle = 0;
        int boneLength = 50;
        if (left == 0 || right == 0)
        {
            rotateAngle = Mathf.Sin(left / boneLength);
        } else
        {
            rotateAngle = Mathf.Sin(halfDistance / boneLength);
        } 
        float originalDegree = bone.transform.eulerAngles.y;

        if (left > right)
        {
            bone.transform.localRotation = Quaternion.Euler(0f, rotateAngle *100000f, 0f);
            UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + rotateAngle);
        } else
        {
            bone.transform.localRotation = Quaternion.Euler(0f, -rotateAngle *10000f, 0f);
            UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + -rotateAngle);
        }
        
        
    }

}
