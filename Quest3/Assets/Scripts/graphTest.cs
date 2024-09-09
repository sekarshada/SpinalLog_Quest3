using Unity.VisualScripting;
using UnityEngine;
using XCharts;
using XCharts.Runtime; // Ensure this namespace matches your XCharts library

public class LineChartController : MonoBehaviour
{
    
    [SerializeField]
    private SpinalLogBluetoothManager BTManager;
    public LineChart lineChart; // Reference to your LineChart component

    // Example data arrays
    private float[] timeData = new float[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
    private float[] forceData = new float[8] { 10, 20, 15, 25, 30, 10, 5, 20 };

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
        if (BTManager.BTHelper.Available) {
            lineChart.AddData(0, BTManager.forceSum);
            lineChart.RefreshChart();
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

        xAxis.splitNumber = 1500;
        xAxis.boundaryGap = false;
     
    }

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
        
        
    }

}
