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
    private float yaxis_force;
    private float smoothedForce = 0f;
    private float baselineForce = 0f;
    private bool baselineSet = false;
    private float studentMin = float.MaxValue;
    private float studentMax = float.MinValue;
    private float expertMin = float.MaxValue;
    private float expertMax = float.MinValue;
    private float counter = 0f;
    private float timer = 0f;
    private float interval = 30f;
    private float last_draw_time = 0f;
    private bool isRestart = true;
    // Offset untuk geser baseline studentTrial
    private float studentOffset = 0f;
    private bool offsetSet = false;
    private string csvFilePath;

    private float studentBaseline = 0f;
private bool studentBaselineSet = false;
    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        csvFilePath = Path.Combine(Application.streamingAssetsPath, "expertTrial2.csv");
#else
        csvFilePath = Path.Combine(Application.streamingAssetsPath, "expertTrial2.csv");
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
        yaxis_force = BTManager.forceSum;
        if (yaxis_force > 1f)
        {
            if (isRestart)
            {
                studentTrial.ClearData();
                isRestart = false;
                baselineSet = false;
                baselineForce = 0f;
                smoothedForce = 0f;
                counter = 0f;
                studentMin = float.MaxValue;
                studentMax = float.MinValue;
                timer = 0f;
                last_draw_time = 0f;
                // reset offset juga setiap restart
                studentOffset = 0f;
                offsetSet = false;
            }
            if (timer < interval && counter < 1500)
            {
                last_draw_time += Time.deltaTime;
                if (last_draw_time >= 0.01f)
                {
                    smoothedForce = Mathf.Lerp(smoothedForce, yaxis_force, 0.2f);
                    if (!baselineSet)
                    {
                        baselineForce = smoothedForce;
                        baselineSet = true;
                        Debug.Log($"Baseline set to: {baselineForce}");
                    }
                    float deltaForce = Mathf.Max(0f, smoothedForce - baselineForce);
                    // Expand student range adaptif
                    if (deltaForce < studentMin) studentMin = deltaForce;
                    if (deltaForce > studentMax) studentMax = deltaForce;
                    if (timer >= 1f) 
                    {
                        float usableRange = Mathf.Max(1e-4f, (studentMax - studentMin) * 1.2f);
                        float normalized = Mathf.Clamp01((deltaForce - studentMin) / usableRange);
                        if (!float.IsFinite(expertMin) || !float.IsFinite(expertMax))
                        {
                            Debug.LogWarning("Expert range invalid. Check CSV parsing.");
                        }
                        else
                        {
                            float adjusted = Mathf.Lerp(expertMin, expertMax, normalized);
                            // Set offset di titik pertama plotting
                            if (!offsetSet)
                            {
                                studentOffset = adjusted;
                                offsetSet = true;
                                Debug.Log($"Student offset set to: {studentOffset}");
                            }
                                if (!studentBaselineSet)
                                {
                                    studentBaseline = adjusted;
                                    studentBaselineSet = true;
                                }
                                float adjustedZeroed = (adjusted - studentBaseline) * -1;
                                studentTrial.AddXYData(counter, adjustedZeroed);
                            //    float adjustedZeroed = Mathf.Max(0f, adjusted - expertMin);
        

                            if (float.IsFinite(adjustedZeroed))
                            {
                                studentTrial.AddXYData(counter, adjustedZeroed);
                                Debug.Log($"deltaForce: {deltaForce}, studentMin: {studentMin}, studentMax: {studentMax}, usableRange: {usableRange}, normalized: {normalized}, adjusted: {adjusted}, studentOffset: {studentOffset}, adjustedZeroed: {adjustedZeroed}");
                               
                               
                                counter += 1f;
                                if (((int)counter) % 200 == 0)
                                    lineChart.RefreshChart();
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"[Learning Range] Min: {studentMin}, Max: {studentMax}");
                    }
                    last_draw_time = 0f;
                }
                
                timer += Time.deltaTime;
            }
            else
            {
                isRestart = true;
                timer = 0f;
                counter = 0f;
            }
        }
        else
        {
            isRestart = true;
            timer = 0f;
            counter = 0f;
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
        xAxis.max = 1000;
        yAxis.show = true;
        yAxis.type = Axis.AxisType.Value;
        yAxis.minMaxType = Axis.AxisMinMaxType.Default;
    }
    void LoadDataFromCSV(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("CSV file not found: " + path);
            return;
        }
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length && i < 1000; i++)
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