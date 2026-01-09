using System.Collections;
using UnityEngine;
using XCharts.Runtime;
using System.IO;
using System;
using System.Globalization;

public class SpinalLogGraph : MonoBehaviour
{
    [SerializeField] private SpinalLogBluetoothManager BTManager;

    public LineChart lineChart;
    public GameObject Graph;

    private Line studentTrial;
    private Line expertTrial;

    // Initial baseline from the very first reading
    private float initialForce = 0f;
    private bool initialSet = false;

    // Smoothed delta (difference) for plotting
    private float smoothedDelta = 0f;

    // Simple plot control
    private float counter = 0f;
    private float lastDrawTime = 0f;

    // Optional: session window to avoid unbounded X
    private float sessionSeconds = 30f;
    private float timer = 0f;

    // Expert CSV range (kept if you still want to show it)
    private float expertMin = float.MaxValue;
    private float expertMax = float.MinValue;
    private string csvFilePath;

    private bool isRestart = true;

    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // csvFilePath = Path.Combine(Application.streamingAssetsPath, "expertTrial2.csv");
        // csvFilePath = Path.Combine(Application.persistentDataPath, "ascending.csv");
        csvFilePath = Path.Combine(Application.persistentDataPath, "descending.csv");
#else
        // csvFilePath = Path.Combine(Application.streamingAssetsPath, "expertTrial2.csv");
        // csvFilePath = Path.Combine(Application.streamingAssetsPath, "ascending.csv");
        csvFilePath = Path.Combine(Application.streamingAssetsPath, "descending.csv");
#endif
        SetupChart();

        lineChart.RemoveData();

        expertTrial = lineChart.AddSerie<Line>("expertTrial");
        expertTrial.lineStyle.width = 3f;
        expertTrial.itemStyle.color = Color.blue;
        expertTrial.symbol.type = SymbolType.None;

        studentTrial = lineChart.AddSerie<Line>("studentTrial");
        studentTrial.lineStyle.width = 3f;
        studentTrial.itemStyle.color = Color.green;
        studentTrial.symbol.type = SymbolType.None;

        LoadDataFromCSV(csvFilePath);
    }

    void Update()
    {
        if (BTManager == null)
        {
            Debug.LogError("SpinalLogBluetoothManager is not initialized.");
            return;
        }

        // (1) Read current force
        float currentForce = BTManager.forceSum;

        // Reset session bookkeeping on restart
        if (isRestart)
        {
            studentTrial.ClearData();
            initialSet = false;
            initialForce = 0f;
            smoothedDelta = 0f;
            counter = 0f;
            timer = 0f;
            lastDrawTime = 0f;
            isRestart = false;
        }

        // (2) Capture the initial baseline from the very first sample
        if (!initialSet)
        {
            initialForce = currentForce;     // store initial value
            initialSet = true;
            smoothedDelta = 2.0f;              // start smoothing from 2.0
            Debug.Log($"Initial baseline captured: {initialForce}");
        }

        // (3) Compute absolute difference from the initial baseline
        float rawDelta = Mathf.Abs(currentForce - initialForce) * 2.0f; // scale factor to enhance visibility

        // (4) Smooth the difference, then plot
        // Use exponential smoothing (tweak alpha if you want more/less smoothing)
        float alpha = 0.1f;
        smoothedDelta = Mathf.Lerp(smoothedDelta, rawDelta, alpha);

        // Draw at ~100 Hz (every 0.01s) to match your original cadence
        lastDrawTime += Time.deltaTime;
        // if (lastDrawTime >= 0.01f && initialSet && counter < 1500) 
        // {
        //     studentTrial.AddXYData(counter, smoothedDelta);
        //     counter += 1f;

        //     // Refresh occasionally for performance
        //     if (((int)counter) % 200 == 0)
        //         lineChart.RefreshChart();

        //     lastDrawTime = 0f;
        // }

        // Optional: keep each session to a window (e.g., 30s), then restart cleanly
        timer += Time.deltaTime;
        if (timer >= sessionSeconds)
        {
            isRestart = true;
        }
    }

    void SetupChart()
    {
        var title = lineChart.EnsureChartComponent<Title>();
        title.text = "Force Over Time";
        var tooltip = lineChart.EnsureChartComponent<Tooltip>();
        tooltip.show = true;

        var legend = lineChart.EnsureChartComponent<Legend>();
        legend.show = true;

        var xAxis = lineChart.EnsureChartComponent<XAxis>();
        var yAxis = lineChart.EnsureChartComponent<YAxis>();

        xAxis.show = true;
        xAxis.type = Axis.AxisType.Value;
        xAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        xAxis.min = 0;
        xAxis.max = 1500; 
        yAxis.show = true;
        yAxis.type = Axis.AxisType.Value;
        yAxis.minMaxType = Axis.AxisMinMaxType.Default; // auto-scale for delta
    }

    void LoadDataFromCSV(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("CSV file not found (expert series will be empty): " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length && i < 1500; i++)
        {
            var parts = lines[i].Split(new[] { ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0 && float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
            {
                expertTrial.AddXYData(i - 1, y);
                if (y < expertMin) expertMin = y;
                if (y > expertMax) expertMax = y;
            }
            else
            {
                Debug.LogWarning($"Could not parse line {i + 1}: {lines[i]}");
            }
        }
        Debug.Log($"Expert Range → Min: {expertMin}, Max: {expertMax}");
    }

    public void showGraph() => Graph.SetActive(true);
    public void hideGraph() => Graph.SetActive(false);
}
