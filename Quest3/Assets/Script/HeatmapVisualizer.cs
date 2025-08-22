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
    public float intensityScale = 0.1f;     // Boost how "hot" the colors appear
    private float[] hits = new float[64 * 3]; // 32 points max (x, y, intensity)
    private int hitCount = 0;

    private float[] previousFrameValues = new float[99];

    private float decayRate = 0.95f; // Decay rate for heatmap intensity

    public ExperimentController experimentController;
    private bool isSpawned = false;

    public GameObject heatmap;

    void Start()
    {
        transform.localScale = new Vector3(0.15f, 0.12f, 1f); // Adjust to fit the fabric
        if (heatmap != null && heatmapMaterial != null)
        {
            var r = heatmap.GetComponent<Renderer>();
            if (r != null) r.material = heatmapMaterial; // ensure correct material is used
        }
        if (heatmap != null)
        {
            heatmap.transform.localScale = new Vector3(0.15f, 0.12f, 0.001f); // z ~ 0
            heatmap.SetActive(true);
        }
    }

    void Update()
    {
        // single-button toggle behavior
        // if (OVRInput.GetDown(OVRInput.Button.One))
        // {
            // if (!isSpawned && experimentController.condition != 1)
            // {
            //     Spawncube();
            //     Debug.Log("Spawned heatmap cube");
            // }
            // else if (isSpawned)
            // {
                if (serialReader == null || serialReader.normalizedValues == null || serialReader.normalizedValues.Length != 99)
                return;

            ClearHits();

            float planeW = 0.15f;
            float planeH = 0.12f;
            float celWidth = planeW / (float)cols;
            float celHeight = planeH / (float)rows; // use rows (not 8f)

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int index = row * cols + col;
                    float current = serialReader.normalizedValues[index];

                    // exponential smoothing
                    float smoothed = Mathf.Lerp(previousFrameValues[index], current, 1f - decayRate);
                    previousFrameValues[index] = smoothed;

                    if (smoothed > noiseThreshold && hitCount < 32)
                    {
                    // float x = col * celWidth - planeW / 2f + celWidth / 2f;
                    // float y = row * celHeight - planeH / 2f + celHeight / 2f;
                        float x = (float)col / (float)(cols - 1);
                        float y = (float)row / (float)(rows - 1);
                        AddHit(x, y, smoothed * intensityScale);
                    }
                }
            }

            ApplyHits();
            // }
        

        // // when spawned, drive the heatmap EVERY frame
        // if (isSpawned)
        // {
            
        // }
    }

    void AddHit(float x, float y, float intensity)
    {
        if (hitCount >= 32) return;
        hits[hitCount * 3 + 0] = x;
        hits[hitCount * 3 + 1] = y;
        hits[hitCount * 3 + 2] = intensity;
        hitCount++;

        Debug.Log($"Hit added at ({x}, {y}) with intensity {intensity}. Total hits: {hitCount}");
    }

    void ClearHits()
    {
        System.Array.Clear(hits, 0, hits.Length);
        hitCount = 0;
    }

    void ApplyHits()
    {
        Debug.Log($"Applying {hitCount} hits to heatmap material.");
        if (heatmapMaterial != null)
        {
            heatmapMaterial.SetFloatArray("_Hits", hits);
            heatmapMaterial.SetInt("_HitCount", hitCount);
        }
    }

    public void Spawncube()
    {
        if (!heatmap.activeInHierarchy)
        {
            Vector3 handPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            handPosition.x -= 0.1f;
            heatmap.transform.position = handPosition;
            Debug.Log("Heatmap cube spawned at position: " + handPosition);
            heatmap.SetActive(true);
            isSpawned = true;
        }
        else
        {
            Debug.LogError("Prefab or RightHandAnchor is not set.");
        }
    }

    public void DeactivateManager()
    {
        gameObject.SetActive(false);
    }
}
