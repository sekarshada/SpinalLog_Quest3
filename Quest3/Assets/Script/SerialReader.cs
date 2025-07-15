using System;
using System.IO.Ports;
using System.Linq;
using UnityEngine;
public class SerialReader : MonoBehaviour
{
    SerialPort serialPort;
    public int[] sensorValues = new int[99];         // Delta (pressure-only)
    public float[] normalizedValues = new float[99]; // 0–1 normalized values
    private int[] baseline = new int[99];
    private bool baselineInitialized = false;
    // Calibration parameters
    private const float maxDelta = 500f; // Max expected pressure delta
    private const float baselineLerpSpeed = 0.01f; // Slow adaptation if idle
    void Start()
    {
        string[] ports = SerialPort.GetPortNames();
        Debug.Log("Available Ports:");
        foreach (string p in ports)
        {
            Debug.Log(p);
        }
        serialPort = new SerialPort("COM3", 115200); // Change COM3 if needed
        try
        {
            serialPort.Open();
            serialPort.ReadTimeout = 100;
            Debug.Log("Serial port opened.");
            serialPort.DiscardInBuffer();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not open serial port: " + e.Message);
        }


       
        var found = FindObjectOfType<SensorHeatmap>();
        if (found != null)
            Debug.Log(":white_tick: SensorHeatmap is attached to: " + found.gameObject.name);
        else
            Debug.LogWarning(":x: SensorHeatmap is NOT attached to any GameObject.");
    
    }
    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    string line = serialPort.ReadLine().Trim();
                    string[] values = line.Split(',');
                    if (values.Length == 99 && values.All(v => int.TryParse(v, out _)))
                    {
                        int[] raw = Array.ConvertAll(values, int.Parse);
                        if (!baselineInitialized)
                        {
                            for (int i = 0; i < 99; i++)
                            {
                                baseline[i] = raw[i];
                            }
                            baselineInitialized = true;
                            Debug.Log("Baseline initialized: " + string.Join(",", baseline));
                            return;
                        }
                        for (int i = 0; i < 99; i++)
                        {
                            // Live baseline recalibration (only when sensor is idle)
                            if ((raw[i] - baseline[i]) < 10)
                            {
                                baseline[i] = Mathf.RoundToInt(Mathf.Lerp(baseline[i], raw[i], baselineLerpSpeed));
                            }
                            // Delta computation (only positive pressure)
                            int delta = raw[i] - baseline[i];
                            sensorValues[i] = Mathf.Max(0, delta);
                            // Normalize to 0–1
                            normalizedValues[i] = Mathf.Clamp01(sensorValues[i] / maxDelta);
                        }
                        // Optional: log a subset
                        Debug.Log("Normalized: " + string.Join(", ", normalizedValues.Select(v => v.ToString("F2"))));
                    }
                    else
                    {
                        Debug.LogWarning("Malformed serial input: " + line);
                    }
                }
            }
            catch (TimeoutException) { }
            catch (System.Exception e)
            {
                Debug.LogWarning("Serial read error: " + e.Message);
            }
        }
    }
    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}