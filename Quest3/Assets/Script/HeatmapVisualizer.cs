using UnityEngine;
public class HeatmapVisualizer : MonoBehaviour
{
    [Header("Dependencies")]
    public Material heatmapMaterial;
    public SerialReader serialReader;
    [Header("Sensor Grid Settings")]
    public int cols = 11;
    public int rows = 9;// 9 rows for 99 sensors
    [Header("Heatmap Tuning")]
    public float noiseThreshold = 0.05f;   // Ignore very small pressure
    public float intensityScale = 3.0f;    // Boost how "hot" the colors appear
    private float[] hits = new float[32 * 3]; // 32 points max (x, y, intensity)
    private int hitCount = 0;
    void Update()
    {
    

        if (serialReader == null || serialReader.normalizedValues == null || serialReader.normalizedValues.Length != 99)
            return;
        ClearHits();
        // float celWidth = 0.15f / (cols - 1);
        // float celHeight = 0.12f / (rows - 1);
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * rows + col;
                if (index >= serialReader.normalizedValues.Length)
                    continue; // Safety check
                float norm = serialReader.normalizedValues[index];
                if (norm > noiseThreshold)
                {
                    float x = Mathf.Lerp(-1f, 1f, col / (float)(cols - 1));
                    float y = Mathf.Lerp(-1f, 1f, row / (float)(rows  -1));
                    AddHit(x, y, norm * intensityScale);
                }
            }
        }
        ApplyHits();
    }
    void AddHit(float x, float y, float intensity)
    {
        if (hitCount >= 32) return;
        hits[hitCount * 3 + 0] = x;
        hits[hitCount * 3 + 1] = y;
        hits[hitCount * 3 + 2] = intensity;
        hitCount++;
    }
    void ClearHits()
    {
        System.Array.Clear(hits, 0, hits.Length);
        hitCount = 0;
    }
    void ApplyHits()
    {
        if (heatmapMaterial != null)
        {
            heatmapMaterial.SetFloatArray("_Hits", hits);
            heatmapMaterial.SetInt("_HitCount", hitCount);
        }
    }
}