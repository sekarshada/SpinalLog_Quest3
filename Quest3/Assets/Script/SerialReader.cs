using System;
using System.Linq;
using UnityEngine;
public class SerialReader : MonoBehaviour
{
   
    public int[] sensorValues = new int[99];
    public float[] normalizedValues = new float[99];
    private int[] baseline = new int[99];
    private bool baselineInitialized = false;
    private const float maxDelta = 500f;
    private const float baselineLerpSpeed = 0.01f;

    public SpinalLogBluetoothManager bluetoothManager;
    void Start()
    {
        if (bluetoothManager != null && bluetoothManager.BTHelper != null)
        {
            bluetoothManager = FindObjectOfType<SpinalLogBluetoothManager>();
            if (bluetoothManager == null || bluetoothManager.BTHelper == null)
            {
                Debug.LogError("Bluetooth manager or its helper is not set.");
                return;
            }
        }
        Debug.Log("Connected to SerialBluetooth");
    }
    void Update()
    {
        if (bluetoothManager.forceMatrix == null || bluetoothManager.forceMatrix.Length != 99) return;
        int[] raw = bluetoothManager.forceMatrix;

        if (!baselineInitialized)
        {
            Array.Copy(raw, baseline, 99);
            baselineInitialized = true;
            Debug.Log("Baseline initialized.");
        }
        for (int i = 0; i < 99; i++)
        {
            int delta = raw[i] - baseline[i];
            if (delta < 10)
                baseline[i] = Mathf.RoundToInt(Mathf.Lerp(baseline[i], raw[i], baselineLerpSpeed));
            sensorValues[i] = Mathf.Max(0, delta);
            normalizedValues[i] = Mathf.Clamp01(sensorValues[i] / maxDelta);
        }
        // Debug.Log("Normalized[0..4]: " + string.Join(", ", normalizedValues.Take(99).Select(v => v.ToString("F2"))));
    }

}