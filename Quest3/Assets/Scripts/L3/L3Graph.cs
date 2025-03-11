using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using XCharts;
using System.IO;
using XCharts.Runtime;
using System.Diagnostics.Tracing; // Ensure this namespace matches your XCharts library

public class L3Graph : MonoBehaviour
{
    
    [SerializeField]
    //private SpinalLogBluetoothManager BTManager;
    private L3BlueToothManager l3Manager;
    public LineChart lineChart; // Reference to your LineChart component

    private float yaxis_force;
    private float last_draw_time = 0f;
    private float last_record = 0f;
    private float timer = 0f;
    private float interval = 30f;
    private float counter = 0f;
    private bool isPressing = false;
    private bool isRestart = true;
    public GameObject Graph;

    private Line studentTrial;
    private Line expertTrial;


    // Smoothing parameters
    private Queue<float> smoothingWindow = new Queue<float>();
    private const int WINDOW_SIZE = 5;  // Adjust this value to change smoothing amount
    private const float SMOOTHING_FACTOR = 0.2f;  // Adjust between 0 and 1 (higher = more smoothing)
    private float lastSmoothedValue = 0f;


    //private string csvFilePath; // Path to your CSV file
    private string csvFilePath = "Assets/Resources/expertTrial-600.csv"; // Path to your CSV file


    void Awake()
    {
        lineChart = gameObject.GetComponent<LineChart>();
        // Initialize the chart
        SetupChart();
        // Update the chart with data
        lineChart.RemoveData();
        
        expertTrial = lineChart.AddSerie<Line>("expertTrial"); 
        studentTrial = lineChart.AddSerie<Line>("studentTrial");

        // Configure student trial line
        studentTrial.symbol.show = false;
        studentTrial.lineStyle.width = 2;

        LoadDataFromCSV(csvFilePath);
        //studentTrial.symbolType = SymbolType.None;
        
    }

     private float SmoothValue(float newValue)
    {
        // Add new value to window
        smoothingWindow.Enqueue(newValue);
        
        // Keep window size fixed
        if (smoothingWindow.Count > WINDOW_SIZE)
        {
            smoothingWindow.Dequeue();
        }

        // Apply Gaussian-like weighting to the window values
        float smoothedValue = 0f;
        float weightSum = 0f;
        float[] weights = new float[WINDOW_SIZE];
        
        // Create bell curve weights
        int center = smoothingWindow.Count / 2;
        int i = 0;
        foreach (float value in smoothingWindow)
        {
            float distance = Mathf.Abs(i - center);
            float weight = Mathf.Exp(-(distance * distance) / (2 * WINDOW_SIZE));
            weights[i] = weight;
            weightSum += weight;
            i++;
        }

        // Apply weighted average
        i = 0;
        foreach (float value in smoothingWindow)
        {
            smoothedValue += value * (weights[i] / weightSum);
            i++;
        }

        // Blend with previous value for additional smoothing
        smoothedValue = (SMOOTHING_FACTOR * smoothedValue) + 
                       ((1 - SMOOTHING_FACTOR) * lastSmoothedValue);
        
        lastSmoothedValue = smoothedValue;
        return smoothedValue;
    }


    void Update() {

        if (L3BlueToothManager.l3Manager != null)
        {
            // Access the static instance of the manager
            yaxis_force = L3BlueToothManager.l3Manager.numbers[0];
            Debug.Log($"Current force reading: {yaxis_force}");
        }
        else
        {
            Debug.LogError("L3BlueToothManager is not initialized.");
            return;
        }
        
        // start press
        if (yaxis_force > 1) {
            // reateart check
            if (isRestart) {
                
                studentTrial.ClearData();
                smoothingWindow.Clear();  // Clear smoothing window
                lastSmoothedValue = 0f;   // Reset last smoothed value
                isRestart = false;
                last_draw_time = 0f;  // Reset the timer when starting
                counter = 0f;         // Reset the counter
                timer = 0f;          // Reset the overall timer
            }
            // draw graph
            isPressing = true;
            
            if (timer < interval) {

                last_draw_time += Time.deltaTime;  // Accumulate time
                
                
                // update every 0.05 seconds
                if (last_draw_time >= 0.05f) {
                    float timeValue = timer;
                    float smoothedForce = SmoothValue(yaxis_force * 2f);
                    studentTrial.AddData(timeValue, smoothedForce);
                    
                    last_draw_time = 0f;
                    counter++;
                    last_record = smoothedForce;
                    Debug.Log($"Point {counter}: time={timeValue}, force={smoothedForce}");
                } 
                       
                timer += Time.deltaTime;

            } else {
                // out of 30 seconds session, pause
                isPressing = false;
                isRestart = true;
                timer = 0f;
                counter = 0f;
            }
            
        } else {
            // stop pressing, refresh
            isPressing = false;
            isRestart = true;
            timer = 0;
            counter = 0f;
        }
        
    }

    void SetupChart()
    {
        var title = lineChart.EnsureChartComponent<Title>();
        title.text = "Force over Time Graph";

        var tooltip = lineChart.EnsureChartComponent<Tooltip>();
        tooltip.show = true;

        var legend = lineChart.EnsureChartComponent<Legend>();
        legend.show = false;

        var xAxis = lineChart.EnsureChartComponent<XAxis>();
        var yAxis = lineChart.EnsureChartComponent<YAxis>();
        xAxis.show = true;
        yAxis.show = true;
        xAxis.type = Axis.AxisType.Value;
        //xAxis.type = Axis.AxisType.Time;
        xAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        xAxis.min = 0;
        xAxis.max = 30;
        // xAxis.max = 1000;
        //xAxis.interval = 50;
        //xAxis.type = Axis.AxisType.Category;
        yAxis.type = Axis.AxisType.Value;

        xAxis.splitNumber = 0;
        xAxis.boundaryGap = false;
     
    }

     void LoadDataFromCSV(string path)
    {
        Debug.Log("1");
        if (!File.Exists(path))
        {
            Debug.LogError("CSV file not found at: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogError("CSV file must contain at least one header line and one data line.");
            return;
        }

        // Calculate time step for 600 points over 30 seconds
        float timeStep = 30f / 600f;

        // Use the index as X values
        // for (int i = 1; i < 1000; i++) // Start from 1 to skip header
        for (int i = 1; i <= 600 && i < lines.Length; i++) // Start from 1 to skip header
        {
            if (float.TryParse(lines[i], out float y))
            {
                float timePoint = (i - 1) * timeStep;  // Calculate the time for this point
                expertTrial.AddData(timePoint, y);
                Debug.Log($"Adding point at time {timePoint}: {y}");
                // Add data to the chart
                //expertTrial.AddData(i-1, y); // Using index as X value
                //Debug.Log("line: "+lines[i]);
            }
            else
            {
                Debug.LogWarning($"Could not parse value on line {i + 1}: {lines[i]}");
            }
        }
    }



    public void showGraph(){
        Graph.SetActive(true);
    }

    public void hideGraph(){
        Graph.SetActive(false);
    }

}

/*
void Update() {
        // for vartebra
        yaxis_force = BTManager.numbers[0];
        //spinal log test
        //yaxis_force = 235-BTManager.forceSum;
        Debug.Log(yaxis_force);
        //timer += Time.deltaTime;   
        
       //Debug.Log("yaxis_force" + yaxis_force);
        
        // start press
        if (yaxis_force > 0 && BTManager.BTHelper.Available) {
            //isPressing = true;
            // draw graph
            if (counter < interval) {
                
                studentTrial.AddData(counter++, yaxis_force);
                Debug.Log("count" + counter);
                Debug.Log("timer" + timer);
                //lineChart.RefreshChart();

            }
            else{
                //timer = 0;
                counter = 0;
                //lineChart.RemoveData();
                studentTrial.ClearData();
                //lineChart.AddSerie<Line>("line");
            }
        }
    
        
    
        
    }*/