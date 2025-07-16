using System;
using System.IO.Ports;
using System.Linq;
using UnityEngine;
public class SerialReader : MonoBehaviour
{
    SerialPort serialPort;
    string serialBuffer = ""; // :brain: collect bytes safely
    public int[] sensorValues = new int[99];
    public float[] normalizedValues = new float[99];
    private int[] baseline = new int[99];
    private bool baselineInitialized = false;
    private const float maxDelta = 500f;
    private const float baselineLerpSpeed = 0.01f;
    void Start()
    {
        string[] ports = SerialPort.GetPortNames();
        Debug.Log("Available Ports:");
        foreach (string p in ports)
            Debug.Log(p);
        serialPort = new SerialPort("COM3", 115200); // Adjust COM port as needed
        try
        {
            serialPort.Open();
            serialPort.ReadTimeout = 50;
            serialPort.DiscardInBuffer();
            Debug.Log("Serial port opened.");
        }
        catch (Exception e)
        {
            Debug.LogError("Could not open serial port: " + e.Message);
        }
        var found = FindObjectOfType<SensorHeatmap>();
        if (found != null)
            Debug.Log("SensorHeatmap is attached to: " + found.gameObject.name);
        else
            Debug.LogWarning("SensorHeatmap is NOT attached to any GameObject.");
    }
    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen) return;
        try
        {
            serialBuffer += serialPort.ReadExisting();
            while (true)
            {
                int newlineIndex = serialBuffer.IndexOf('\n');
                if (newlineIndex == -1) break;
                string line = serialBuffer.Substring(0, newlineIndex).Trim();
                serialBuffer = serialBuffer.Substring(newlineIndex + 1);
                string[] values = line.Split(',');
                if (values.Length != 99)
                {
                    if (line.Length < 200)
                        Debug.LogWarning($":warning: Malformed serial input (len={values.Length}): {line}");
                    continue;
                }
                int[] raw = new int[99];
                for (int i = 0; i < 99; i++)
                {
                    if (!int.TryParse(values[i], out raw[i]))
                    {
                        Debug.LogWarning($":warning: Failed to parse value at index {i}: '{values[i]}'");
                        raw[i] = 0;
                    }
                }
                if (!baselineInitialized)
                {
                    Array.Copy(raw, baseline, 99);
                    baselineInitialized = true;
                    Debug.Log(" Baseline initialized.");
                }
                for (int i = 0; i < 99; i++)
                {
                    int delta = raw[i] - baseline[i];
                    if (delta < 10)
                        baseline[i] = Mathf.RoundToInt(Mathf.Lerp(baseline[i], raw[i], baselineLerpSpeed));
                    sensorValues[i] = Mathf.Max(0, delta);
                    normalizedValues[i] = Mathf.Clamp01(sensorValues[i] / maxDelta);
                }
                Debug.Log(":bar_chart: Normalized[0..4]: " + string.Join(", ", normalizedValues.Take(5).Select(v => v.ToString("F2"))));
            }
            // Optional: clean up runaway buffer
            if (serialBuffer.Length > 2000)
            {
                Debug.LogWarning(" Serial buffer overflow — clearing.");
                serialBuffer = "";
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Serial read error: " + e.Message);
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