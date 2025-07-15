using System.Security.Cryptography;
using UnityEngine;
public class SensorHeatmap : MonoBehaviour
{
    public Material heatmapMaterial; // Material using a shader that supports heatmap visualisation
    public SerialReader serialReader; // Reference to SerialReader.cs
    private const int cols = 11;
    private const int rows = 9;
    private const int numSensor = rows * cols;
    private float[] heatPoints = new float[numSensor * 3]; // Each point has x, y, intensity
    private int hitCount = 0;
    void Update()
    {
        Debug.Log("SENSOR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.Log($"SensorHeatmap started. GameObject active: {gameObject.activeInHierarchy}, script enabled: {enabled}");

        if (serialReader == null || serialReader.sensorValues == null || serialReader.sensorValues.Length != numSensor)
            return;
        hitCount = 0;

        float aspectRatio = 11f / 15f;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                float norm = serialReader.normalizedValues[index]; // Normalize value between 0–1
                if (norm > 0.05f) // Noise threshold
                {
                    // Map col, row to [-1, 1] range in UV space
                    float x = col / (float)(cols - 1) * 2f - 1f;   // [-1, 1]
                    float y = (1f - row / (float)(rows - 1)) * 2f - 1f; // Y flipped for top-left origin
                    // Adjust for fabric aspect ratio
                    y *= aspectRatio;
                    // Store into array
                    heatPoints[hitCount * 3 + 0] = x;
                    heatPoints[hitCount * 3 + 1] = y;
                    heatPoints[hitCount * 3 + 2] = norm * 3f; // intensity boost if desired
                    hitCount++;
                }
            }
        }

        Debug.Log($"HitCount: {hitCount}");

        heatmapMaterial.SetFloatArray("_Hits", heatPoints);
        heatmapMaterial.SetInt("_HitCount", hitCount);
    }

    
}