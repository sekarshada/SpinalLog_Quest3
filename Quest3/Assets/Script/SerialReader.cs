using System;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Globalization;
public class SerialReader : MonoBehaviour
{
   
    public int[] sensorValues = new int[99];
    public float[] normalizedValues = new float[99];
    private int[] baseline = new int[99];
    private bool baselineInitialized = false;
    private const float maxDelta = 1470f;
    private const float baselineLerpSpeed = 0.01f;
    public float firstRowScale = 0.2f;
    public SpinalLogBluetoothManager bluetoothManager;

     [Header("CSV logging")]
    public float logInterval = 0.05f; // seconds
    private StreamWriter csvWriter;
    private string csvPath;
    private float nextLogTime;
    private bool isLogging;
    public bool saveInScriptsFolderInEditor = true;
    public string scriptsLogSubfolder = "Logs"; // within Assets/Script
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
        Debug.Log("Raw[0..4]: " + string.Join(", ", bluetoothManager.forceMatrix.Take(99).Select(v => v.ToString())));
        if (!baselineInitialized)
        {
            Array.Copy(raw, baseline, 99);
            baselineInitialized = true;
            Debug.Log("Baseline initialized.");
        }
        for (int i = 0; i < 99; i++)
        {
            int delta = raw[i] - baseline[i];
            if (delta < 500)
                baseline[i] = Mathf.RoundToInt(Mathf.Lerp(baseline[i], raw[i], baselineLerpSpeed));
            sensorValues[i] = Mathf.Max(0, delta);
            normalizedValues[i] = Mathf.Clamp01(sensorValues[i] / maxDelta);
        }
        float scale = Mathf.Clamp01(firstRowScale);
        for (int i = 0; i < 11 && i < normalizedValues.Length; i++)
            normalizedValues[i] = Mathf.Clamp01(normalizedValues[i] * scale);


        if (isLogging && Time.time >= nextLogTime)
        {
            nextLogTime = Time.time + logInterval;
            WriteCsvRow();
        }

        // Debug.Log("Normalized[0..4]: " + string.Join(", ", normalizedValues.Take(99).Select(v => v.ToString("F2"))));
        Debug.Log(bluetoothManager.forceMatrix);
    }
    
    // Start logging (creates file and writes header)
    public void StartCsvLogging(string fileSuffix = null)
    {
         if (isLogging) return;

        string file = "normalized_" + (string.IsNullOrEmpty(fileSuffix)
            ? DateTime.Now.ToString("yyyyMMdd_HHmmss")
            : fileSuffix) + ".csv";

        if (saveInScriptsFolderInEditor && Application.isEditor)
        {
            // Assets/Script[/Logs]
            string dir = Path.Combine(Application.dataPath, "Script");
            if (!string.IsNullOrEmpty(scriptsLogSubfolder))
                dir = Path.Combine(dir, scriptsLogSubfolder);
            Directory.CreateDirectory(dir);
            csvPath = Path.Combine(dir, file);
        }
        else
        {
            // Fallback (Editor disabled, Standalone, or Quest)
            Directory.CreateDirectory(Application.persistentDataPath);
            csvPath = Path.Combine(Application.persistentDataPath, file);
        }

        csvWriter = new StreamWriter(csvPath, append: false);
        csvWriter.WriteLine("timestamp," + string.Join(",", Enumerable.Range(0, 99).Select(i => $"n{i}")));
        csvWriter.Flush();
        isLogging = true;
        nextLogTime = Time.time;
        Debug.Log($"CSV logging started: {csvPath}");
    }

    // Stop logging and close file
    public void StopCsvLogging()
    {
        if (!isLogging) return;
        csvWriter?.Flush();
        csvWriter?.Dispose();
        csvWriter = null;
        isLogging = false;
        Debug.Log($"CSV logging stopped: {csvPath}");
    }

    // Write one row immediately (creates file if needed)
    public void LogOneRowNow(string optionalSuffix = null)
    {
        if (!isLogging) StartCsvLogging(optionalSuffix);
        WriteCsvRow();
        csvWriter?.Flush();
    }

    private void WriteCsvRow()
    {
        if (csvWriter == null) return;
        string ts = DateTime.Now.ToString("O", CultureInfo.InvariantCulture);
        string line = ts + "," + string.Join(",", normalizedValues.Select(v => v.ToString("F4", CultureInfo.InvariantCulture)));
        csvWriter.WriteLine(line);
    }

    private void OnDestroy()
    {
        StopCsvLogging();
    }

}