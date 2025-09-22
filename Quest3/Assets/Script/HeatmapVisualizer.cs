using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
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
    private const int maxHits = 99;        // Max points to send to shader
    private float[] hits = new float[maxHits * 3]; // 32 points max (x, y, intensity)
    private int hitCount = 0;

    private float[] previousFrameValues = new float[99];

    private float decayRate = 0.95f; // Decay rate for heatmap intensity

    public ExperimentController experimentController;
    private bool isSpawned = false;

    public GameObject heatmap;
    private Transform grabFunction;

    public Grabbable grabbable;
    public GrabInteractable grabInteractable;

    public HandGrabInteractable handGrabInteractable;
    private Vector3 latestHeatmapPosition;
    private bool firstFrame = true;
    [Header("Visual Size")]
    public float blobDiameter = 0.06f;
    public float blobStrength = 4f;
    void Start()
    {
        transform.localScale = new Vector3(0.15f, 0.12f, 1f); // Adjust to fit the fabric
        if (heatmap != null && heatmapMaterial != null)
        {
            var r = heatmap.GetComponent<Renderer>();
            if (r != null)
            {
                r.material = heatmapMaterial; // ensure correct material is used
                var mf = heatmap.GetComponent<MeshFilter>();

                if (heatmapMaterial)
                {
                    heatmapMaterial.SetVector("_LocalMin", new Vector4(-0.075f, -0.06f, 0, 0));
                    heatmapMaterial.SetVector("_LocalMax", new Vector4(0.075f, 0.06f, 0, 0));
                }

        if (mf && mf.sharedMesh)
                {
                    var b = mf.sharedMesh.bounds;                   // mesh-local bounds (pre-transform)
                    var size = b.size;                              // span in local X/Y/Z
                    bool useXY = /* same as your material toggle */ true; // or false for XZ

                    var span = useXY ? new Vector2(size.x, size.y) : new Vector2(size.x, size.z);
                    Vector4 localMin = r.sharedMaterial.GetVector("_LocalMin");
                    Vector4 localMax = r.sharedMaterial.GetVector("_LocalMax");
                    Debug.Log($"[Heatmap] Mesh local span {(useXY ? "XY" : "XZ")} = {span}");
                    Debug.Log($"[Heatmap] Shader LocalMin = {localMin}, LocalMax = {localMax} (check in material)");

                    // Optional: compare aspect vs your physical matrix (e.g., 0.15m x 0.12m)
                    float meshAspect = span.x / Mathf.Max(1e-6f, span.y);
                    float physAspect = 0.15f / 0.12f; // your fabric width/height if that's correct
                    Debug.Log($"[Heatmap] Mesh aspect={meshAspect:F3} vs Physical aspect={physAspect:F3}");
                }

            }
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
            // float maxRaw = 0f;
            // for (int row = 0; row < rows; row++)
            // {
            //     for (int col = 0; col < cols; col++)
            //     {
            //         int idx = row * cols + col;
            //         float val = serialReader.normalizedValues[idx];
            //         if (val > maxRaw) maxRaw = val;
            //     }
            // }
            // Debug.Log($"[Heatmap] MaxRawFrame={maxRaw:F3}");

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
                // float smoothed = Mathf.Lerp(previousFrameValues[index], current, 1f - decayRate);
                float smoothed;
                if (firstFrame) 
                {
                    smoothed = current;
                }
                else
                {
                    smoothed = Mathf.Lerp(previousFrameValues[index], current, 1f - decayRate);
                }
                previousFrameValues[index] = smoothed;

                    if (smoothed > noiseThreshold && hitCount < maxHits)
                    {
                    // float x = col * celWidth - planeW / 2f + celWidth / 2f;
                    // float y = row * celHeight - planeH / 2f + celHeight / 2f;
                       float x = col / (float)(cols - 1);         // [0,1]
                        float y = 1f - row / (float)(rows - 1);    // [0,1] top->bottom
                        // no aspect scaling here

                        AddHit(x, y, smoothed * intensityScale);
                    }
                }
            }
        if (hitCount == 0)
        {
                Debug.Log("[Heatmap] No hits passed threshold this frame.");
                // AddHit(0.5f, 0.5f, 1f); // debug forced point
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
        if (hitCount >= maxHits) return;
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
        if (heatmap == null) return;
        var renderer = heatmap.GetComponent<Renderer>();
        if (!renderer) return;

        var mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);

        mpb.SetFloatArray("_Hits", hits);
        mpb.SetInt("_HitCount", hitCount);
        mpb.SetFloat("_PulseSpeed", 0f);
        mpb.SetFloat("_Diameter", blobDiameter);
        mpb.SetFloat("_Strength", blobStrength);

        renderer.SetPropertyBlock(mpb);
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

        public void FixPosition() {
        Debug.Log("clickedHeatMapFix");
    
        // Fix the position of heatmapplane (example: set to origin, or any desired position)
        if (heatmap != null)
        {
            // Vector3 handPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            // handPosition.x -= 0.1f;
            // heatmap.transform.position = handPosition;
            // Debug.Log("Heatmap position fixed at: " + heatmap.transform.position);
    
            
            var handGrab = heatmap.GetComponent<HandGrabInteractable>();
            if (handGrab != null)
            {
                handGrab.enabled = false;
                Debug.Log("HandGrabInteractable disabled.");
            }
    
            var grabbable = heatmap.GetComponent<Grabbable>();
            if (grabbable != null)
            {
                grabbable.enabled = false;
                Debug.Log("Grabbable disabled.");
            }
    
            var grabInteractable = heatmap.GetComponent<GrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
                Debug.Log("GrabInteractable disabled.");
            }
        }
        else
        {
            Debug.LogWarning("Heatmap GameObject not assigned!");
        }
    }

    public void DeactivateManager()
    {
        gameObject.SetActive(false);
    }
}
