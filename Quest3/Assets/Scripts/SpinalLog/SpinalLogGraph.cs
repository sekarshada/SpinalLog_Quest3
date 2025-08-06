using System.Collections;
using UnityEngine;
using XCharts.Runtime;
using System.IO;
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
    private string csvFilePath;
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
        if (yaxis_force > 1)
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
            }
            if (timer < interval)
            {
                if (last_draw_time >= 0.01f)
                {
                    smoothedForce = Mathf.Lerp(smoothedForce, yaxis_force, 0.2f);
                    if (!baselineSet)
                    {
                        baselineForce = smoothedForce;
                        baselineSet = true;
                        Debug.Log($"Baseline set to: {baselineForce}");
                    }
                    float deltaForce = Mathf.Max(0f, yaxis_force - baselineForce);
                    // Learn student min/max in first second
                   if (timer < 1f)
                    {
                        float learnDelta = Mathf.Max(0f, yaxis_force - baselineForce);
                        smoothedForce = Mathf.Lerp(smoothedForce, learnDelta, 0.2f);
                        if (smoothedForce < studentMin) studentMin = smoothedForce;
                        if (smoothedForce > studentMax) studentMax = smoothedForce;
                        Debug.Log($"[Learning Range] Min: {studentMin}, Max: {studentMax}");
                    }
                    else
                    {
                        float usableRange = studentMax - studentMin;
                        if (usableRange < 0.5f) usableRange = 0.5f; // Prevent flat-line
                        float normalized = (deltaForce - studentMin) / usableRange;
                        float scaled = expertMin + normalized * (expertMax - expertMin);
                        studentTrial.AddData(counter++, scaled);
                        Debug.Log($"Delta: {deltaForce}, Normalized: {normalized}, Scaled: {scaled}");
                    }
                    last_draw_time = 0f;
                }
                else
                {
                    last_draw_time += Time.deltaTime;
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
            if (float.TryParse(lines[i], out float y))
            {
                expertTrial.AddData(i - 1, y);
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





