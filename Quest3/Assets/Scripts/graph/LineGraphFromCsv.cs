using UnityEngine;
using System.Collections;
using System.IO;
using XCharts;
using XCharts.Runtime;

public class LineGraphFromCSV : MonoBehaviour
{
    public string csvFilePath = "Assets/Resources/expertTrial.csv"; // Path to your CSV file
    private LineChart lineChart;

    void Start()
    {
        lineChart = GetComponent<LineChart>();
    
        LoadDataFromCSV(csvFilePath);
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
                lineChart.AddData(0, y); // Using index as X value
                Debug.Log("line: "+lines[i]);
            }
            else
            {
                Debug.LogWarning($"Could not parse value on line {i + 1}: {lines[i]}");
            }
        }
    }

    

}
