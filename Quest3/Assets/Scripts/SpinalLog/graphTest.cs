using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using XCharts;
using XCharts.Runtime; // Ensure this namespace matches your XCharts library

public class LineChartController : MonoBehaviour
{
    
    [SerializeField]
    private SpinalLogBluetoothManager BTManager;
    public LineChart lineChart; // Reference to your LineChart component

    private float yaxis_force;
    private float timer = 0f;
    private float interval = 30f;

    // Example data arrays
    private float[] timeData = new float[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
    private float[] forceData = new float[8] { 10, 20, 15, 25, 30, 10, 5, 20 };

    private int counter = 0;
    private bool isPressing = false;
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
        
        yaxis_force = 235-BTManager.forceSum;
        Debug.Log(yaxis_force);
        timer += Time.deltaTime;   
        
       //Debug.Log("yaxis_force" + yaxis_force);
        
        // start press
        if (yaxis_force > 5 && BTManager.BTHelper.Available) {
            //isPressing = true;
            // draw graph
            if (timer < interval) {
                
                lineChart.AddData(0, yaxis_force);
                lineChart.RefreshChart();

            }
            else{
                timer = 0f;
                lineChart.RemoveData();
                lineChart.AddSerie<Line>("line");
            }
        }
        /*
        else{
            //isPressing = false;
            timer = 0f;
            lineChart.RemoveData();
            lineChart.AddSerie<Line>("line");
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


    public void showGraph(){
        Graph.SetActive(true);
    }

    public void hideGraph(){
        Graph.SetActive(false);
    }

}

/*
// start press
        if (yaxis_force > 5) {
            isPressing = true;
            // draw graph
            if (timer < interval) {
                
                lineChart.AddData(0, yaxis_force);
                lineChart.RefreshChart();

            }
            else{
                timer = 0f;
                lineChart.RemoveData();
                lineChart.AddSerie<Line>("line");
            }
        }
        else{
            isPressing = false;
            timer = 0f;
            lineChart.RemoveData();
            lineChart.AddSerie<Line>("line");
        }

        if (isPressing && timer < interval){
            lineChart.AddData(0, yaxis_force);
            lineChart.RefreshChart();
        }*/