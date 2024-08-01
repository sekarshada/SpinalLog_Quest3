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
            BTHelper = BluetoothHelper.GetInstance("ESP32-SpinalLog-Antony");
            if (BTHelper.isDevicePaired())
                if (BTHelper.isConnected())
                    BTHelper.StartListening();
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

    // Update is called once per frame
    void Update()
    {
        if (BTHelper != null)
        {
            if (BTHelper.Available)
            {
                message = BTHelper.Read();
                Console.WriteLine(message);
                Debug.Log(message);
            }
        }
    }

    void OnGUI()
    {

        if (BTHelper == null)
            return;


        BTHelper.DrawGUI();

        if (!BTHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Connect"))
            {
                if (BTHelper.isDevicePaired())
                    BTHelper.Connect(); // tries to connect
            }

        if (BTHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height - 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                BTHelper.Disconnect();
            }
    }

    void OnDestroy()
    {
        if (BTHelper != null)
            BTHelper.Disconnect();
    }

}
