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
    public float noiseThreshold = 0.15f;    // Ignore very small pressure
    public float intensityScale = 0.8f;     // Boost how "hot" the colors appear
    private float[] hits = new float[32 * 3]; // 32 points max (x, y, intensity)
    private int hitCount = 0;

    private float[] previousFrameValues = new float[99];

    private float decayRate = 0.95f; // Decay rate for heatmap intensity

    void Start()
    {
        transform.localScale = new Vector3(0.15f, 0.12f, 1f); // Adjust to fit the fabric
    }
    void Update()
    {
    

        if (serialReader == null || serialReader.normalizedValues == null || serialReader.normalizedValues.Length != 99)
            return;
        ClearHits();
        float celWidth = 0.15f / 11f;
        float celHeight = 0.12f / 8f; // 8 rows for 9 sensors
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                if (index >= serialReader.normalizedValues.Length)
                    continue; // Safety check
               
               if (index >= serialReader.normalizedValues.Length)
                    continue;
                float current = serialReader.normalizedValues[index];
                // Apply decay: blend current with previous
                float smoothed = Mathf.Lerp(previousFrameValues[index], current, 1f - decayRate);
                previousFrameValues[index] = smoothed;
                if (smoothed > noiseThreshold)
                {
                    float x = col * celWidth - 0.15f / 2f + celWidth / 2f;
                    float y = row * celHeight - 0.12f / 2f + celHeight / 2f;
                    AddHit(x, y, smoothed * intensityScale);
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