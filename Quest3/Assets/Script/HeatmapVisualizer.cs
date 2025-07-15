using UnityEngine;
public class HeatmapVisualizer : MonoBehaviour
{
    [Header("Dependencies")]
    public Material heatmapMaterial;
    public SerialReader serialReader;
    [Header("Sensor Grid Settings")]
    public int gridSize = 4; // 4x4 matrix
    [Header("Heatmap Tuning")]
    public float noiseThreshold = 0.05f;   // Ignore very small pressure
    public float intensityScale = 3.0f;    // Boost how "hot" the colors appear
    private float[] hits = new float[32 * 3]; // 32 points max (x, y, intensity)
    private int hitCount = 0;
    void Update()
    {
        // :white_tick: Safety checks
        if (serialReader == null || serialReader.normalizedValues == null || serialReader.normalizedValues.Length != 16)
            return;
        ClearHits();
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                int index = row * gridSize + col;
                float norm = serialReader.normalizedValues[index];
                if (norm > noiseThreshold)
                {
                    float x = Mathf.Lerp(-1f, 1f, col / (float)(gridSize - 1));
                    float y = Mathf.Lerp(-1f, 1f, row / (float)(gridSize - 1));
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