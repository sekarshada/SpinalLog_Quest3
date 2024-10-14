using System.Collections;
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

    private string csvFilePath = "Assets/Resources/expertTrial2.csv"; // Path to your CSV file


    void Start()
    {
        lineChart = gameObject.GetComponent<LineChart>();
        // Initialize the chart
        SetupChart();
        // Update the chart with data
        lineChart.RemoveData();
        expertTrial = lineChart.AddSerie<Line>("expertTrial"); 
        studentTrial = lineChart.AddSerie<Line>("studentTrial");
      
        LoadDataFromCSV(csvFilePath);
        //studentTrial.symbolType = SymbolType.None;
        
    }

    void Update() {

        if (L3BlueToothManager.l3Manager != null)
        {
            // Access the static instance of the manager
            yaxis_force = L3BlueToothManager.l3Manager.numbers[0];
            //Debug.Log("Y-axis Force: " + yaxis_force); 
        }
        else
        {
            Debug.LogError("L3BlueToothManager is not initialized.");
        }
        
        // start press
        if (yaxis_force > 1) {
            // reateart check
            if (isRestart) {
                studentTrial.ClearData();
                isRestart = false;
            }
            // draw graph
            isPressing = true;
            
            if (timer < interval) {
                
                
                // update every 0.01 seconds
                if (last_draw_time >= 0.01) {
                    studentTrial.AddData(counter++, yaxis_force*2f);
                    Debug.Log("force = " + yaxis_force*2f);
                    last_draw_time = 0f;
                    last_record = yaxis_force;
                } else {
                    last_draw_time += Time.deltaTime;
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
        xAxis.max = 1500;
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

        // Use the index as X values
        for (int i = 1; i < lines.Length; i++) // Start from 1 to skip header
        {
            if (float.TryParse(lines[i], out float y))
            {
                // Add data to the chart
                expertTrial.AddData(i-1, y); // Using index as X value
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