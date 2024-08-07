using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;

public class MyBlueToothManager : MonoBehaviour
{
    string message;
    private BluetoothHelper BTHelper;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            //Debug.Log("2");
            BTHelper = BluetoothHelper.GetInstance("ESP32-SpinalLog-Antony"); //device name
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
                Debug.Log(message);
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
                    Debug.Log("Connected!!!");
            }

        if (BTHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height - 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                BTHelper.Disconnect();
                Debug.Log("DisConnected.");
            }
    }*/

    void OnDestroy()
    {
        if (BTHelper != null)
            BTHelper.Disconnect();
    }

    public void connectBT() {
        if (!BTHelper.isConnected()) {
            if (BTHelper.isDevicePaired()) {
                BTHelper.Connect(); // tries to connect
                Debug.Log("Connected!!!");
            }
        }       
    }

    public void disconnectBT() {
        if (BTHelper.isConnected()) {
            BTHelper.Disconnect();
            Debug.Log("DisConnected.");
        }       
    }

}
