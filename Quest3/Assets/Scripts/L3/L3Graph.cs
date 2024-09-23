using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using XCharts;
using XCharts.Runtime; // Ensure this namespace matches your XCharts library

public class L3Graph : MonoBehaviour
{
    
    [SerializeField]
    private L3BlueToothManager BTManager;
    public LineChart lineChart; // Reference to your LineChart component

    private float yaxis_force;
    private float timer = 0f;
    private float interval = 30f;

    // Example data arrays
    private float[] timeData = new float[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
    private float[] forceData = new float[8] { 10, 20, 15, 25, 30, 10, 5, 20 };

    private int counter = 0;
    private bool isFirstPress = true;
    public GameObject Graph;


    void Start()
    {
        lineChart = gameObject.GetComponent<LineChart>();
        // Initialize the chart
        SetupChart();
        // Update the chart with data
        lineChart.RemoveData();
        lineChart.AddSerie<Line>("line");

        //UpdateChart();
    }

    void Update() {
        yaxis_force = BTManager.numbers[0];
        //timer += Time.deltaTime;   
        
       //Debug.Log("yaxis_force" + yaxis_force);
        
        // start press
        if (yaxis_force > 0 && BTManager.BTHelper.Available) {
            //timer += Time.deltaTime;
            //Debug.Log(timer);
        
        
            // clean up graph for each try
            if (isFirstPress) {
                lineChart.RemoveData();
                lineChart.AddSerie<Line>("line");
                isFirstPress = false;
                lineChart.AddData(0, 0);
                lineChart.RefreshChart();
            }
            
            //Debug.Log("yaxis_force" + yaxis_force + "counter" + counter +"timer" + timer);
            // draw graph
            if (timer <= interval) {
                
                lineChart.AddData(0, yaxis_force);
                lineChart.RefreshChart();

                //counter++;
                //timer = 0f;

            }
            
        } 
        else if (yaxis_force < 0 && !isFirstPress){
            //Debug.Log("hhhhh");
            //Debug.Log(yaxis_force);
            lineChart.AddData(0, 0);
            lineChart.RefreshChart();

            timer = 0f;
            counter = 0;
            isFirstPress = true;
        }



        /*if (BTManager.BTHelper.Available && timer<interval) {
            yaxis_force = 235-BTManager.forceSum;
            Debug.Log("yaxis_force" + yaxis_force);
            lineChart.AddData(0, yaxis_force);
            lineChart.RefreshChart();
        }
        else if(timer >= interval){
            lineChart.RemoveData();
            lineChart.AddSerie<Line>("line");
            timer = 0f;
        }*/
        
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
        //xAxis.interval = 50;
        //xAxis.type = Axis.AxisType.Category;
        yAxis.type = Axis.AxisType.Value;

        xAxis.splitNumber = 30;
        xAxis.boundaryGap = false;
     
    }
/*
    void UpdateChart()
    {
        lineChart.RemoveData();
        lineChart.AddSerie<Line>("line");
        if (BTManager.BTHelper.Available)
        {
            Debug.Log("connected!");
            // Add X-axis labels (time data)
            for (int i = 0; i < timeData.Length; i++)
            {
                lineChart.AddXAxisData(timeData[i].ToString()); // Add time data as X-axis labels
            }

            // Add Force data
            for (int i = 0; i < forceData.Length; i++)
            {
                Debug.Log(i + ", FORCE: " + BTManager.forceSum);
                lineChart.AddData(0, forceData[i]); // Add force data to the first series
            }

            // Refresh the chart to update the display
            lineChart.RefreshChart();

        } 
        
        
    }*/

    public void showGraph(){
        Graph.SetActive(true);
    }

    public void hideGraph(){
        Graph.SetActive(false);
    }

}
