using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class HeatmapVisualizerExpert : MonoBehaviour
{
    [Header("Dependencies")]
    public Material heatmapMaterial;
    public GameObject heatmap;

    [Header("Sensor Grid Settings")]
    public int cols = 11;
    public int rows = 9; // 9 x 11 = 99

    [Header("Visual Tuning")]
    public float intensityScale = 10f;
    public float blobDiameter = 0.1f;
    public float blobStrength = 10f;
    [Range(0f, 1f)]
    public float smoothing = 0.05f;

    private const int maxHits = 99;
    private readonly float[] hits = new float[maxHits * 3];
    private int hitCount = 0;
    private readonly float[] previousFrameValues = new float[99];
    private bool firstFrame = true;

    [Header("CSV Playback")]
    public bool useCsvPlayback = true;
    public string csvPath;
    public bool useCsvTimestamps = true;
    public bool loopPlayback = true;
    public float playbackFpsFallback = 20f;

    private readonly List<float[]> csvFrames = new List<float[]>();
    private readonly List<double> csvTimes = new List<double>();
    private int csvFrameIndex = 0;
    private double csvDuration = 0.0;
    private double playbackStartTime = 0.0;

    [Header("Mapping Debug")]
    public bool cornerTest = false;
    public bool useLocalSpaceHits = true;          // send local-space coords (meters)
    public float localMinX = -0.075f, localMaxX = 0.075f;
    public float localMinY = -0.06f,  localMaxY = 0.06f;

    [Header("CSV -> Grid Mapping Options")]
    public bool csvRowMajor = true;                // true: index = r*cols + c; false: index = c*rows + r
    public bool csvSerpentine = false;             // reverse every other row (if row-major)
    public bool serpentineStartsLeftToRight = true;// if true: row0 L->R, row1 R->L, ...
    public bool flipX = false;                     // mirror across X (columns)
    public bool flipY = false;                     // mirror across Y (rows)

    void Start()
    {
        transform.localScale = new Vector3(0.15f, 0.12f, 1f);

        if (heatmap != null && heatmapMaterial != null)
        {
            var r = heatmap.GetComponent<Renderer>();
            if (r != null)
            {
                r.material = heatmapMaterial;
                heatmapMaterial.SetVector("_LocalMin", new Vector4(localMinX, localMinY, 0, 0));
                heatmapMaterial.SetVector("_LocalMax", new Vector4(localMaxX, localMaxY, 0, 0));
            }

            heatmap.transform.localScale = new Vector3(0.15f, 0.12f, 0.001f);
            heatmap.SetActive(true);
        }

        if (useCsvPlayback)
        {
            if (!string.IsNullOrEmpty(csvPath) && File.Exists(csvPath))
                LoadCsv(csvPath);
            else
                LoadLatestCsvFrom(Path.Combine(Application.dataPath, "Script", "Logs"));

            ResetPlayback();
        }
    }

    void Update()
    {
        if (cornerTest)
        {
            ClearHits();
            AddHitUV(0f, 1f, 1f * intensityScale); // top-left
            AddHitUV(1f, 1f, 1f * intensityScale); // top-right
            AddHitUV(0f, 0f, 1f * intensityScale); // bottom-left
            AddHitUV(1f, 0f, 1f * intensityScale); // bottom-right
            ApplyHits();
            return;
        }

        if (!useCsvPlayback || csvFrames.Count == 0) return;

        // Playback time -> frame select
        double t = Time.timeAsDouble - playbackStartTime;
        if (loopPlayback && csvDuration > 0.0) t %= csvDuration;
        if (t < 0) t = 0;
        while (csvFrameIndex + 1 < csvFrames.Count && csvTimes[csvFrameIndex + 1] <= t)
            csvFrameIndex++;

        var src = csvFrames[csvFrameIndex];

        ClearHits();

        // Map all 99 cells
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int csvIdx = ComputeCsvIndex(row, col);
                float current = src[csvIdx];

                float smoothed = firstFrame
                    ? current
                    : Mathf.Lerp(previousFrameValues[csvIdx], current, Mathf.Clamp01(smoothing));
                previousFrameValues[csvIdx] = smoothed;

                // Output column/row after flips
                int outCol = flipX ? (cols - 1 - col) : col;
                int outRow = flipY ? (rows - 1 - row) : row;

                float u = outCol / (float)(cols - 1);
                float v = 1f - outRow / (float)(rows - 1);

                AddHitUV(u, v, smoothed * intensityScale);
            }
        }
        if (firstFrame) firstFrame = false;

        ApplyHits();
        if (!loopPlayback && csvFrameIndex >= csvFrames.Count - 1) enabled = false;
    }

    // Map plane (row,col) to CSV index according to layout options
    int ComputeCsvIndex(int row, int col)
    {
        if (csvRowMajor)
        {
            int c = col;
            if (csvSerpentine)
            {
                bool rowIsLR = (row % 2 == 0) == serpentineStartsLeftToRight;
                if (!rowIsLR) c = cols - 1 - col;
            }
            return row * cols + c;
        }
        else
        {
            // column-major: contiguous by column, stepping rows first
            int r = row;
            // Optional serpentine by column (rare). If needed, adapt here similarly.
            return col * rows + r;
        }
    }

    // Add a hit in UV (0..1). Converted to local if useLocalSpaceHits is true.
    void AddHitUV(float u, float v, float intensity)
    {
        if (hitCount >= maxHits) return;

        if (useLocalSpaceHits)
        {
            float x = Mathf.Lerp(localMinX, localMaxX, u);
            float y = Mathf.Lerp(localMinY, localMaxY, v);
            hits[hitCount * 3 + 0] = x;
            hits[hitCount * 3 + 1] = y;
            hits[hitCount * 3 + 2] = intensity;
        }
        else
        {
            hits[hitCount * 3 + 0] = u;
            hits[hitCount * 3 + 1] = v;
            hits[hitCount * 3 + 2] = intensity;
        }
        hitCount++;
    }

    void ClearHits()
    {
        Array.Clear(hits, 0, hits.Length);
        hitCount = 0;
    }

    void ApplyHits()
    {
        if (heatmap == null) return;
        var renderer = heatmap.GetComponent<Renderer>();
        if (!renderer) return;

        var mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);

        mpb.SetFloatArray("_Hits", hits);   // triplets (x, y, intensity)
        mpb.SetInt("_HitCount", hitCount);
        mpb.SetFloat("_PulseSpeed", 0f);
        mpb.SetFloat("_Diameter", blobDiameter);
        mpb.SetFloat("_Strength", blobStrength);

        renderer.SetPropertyBlock(mpb);
    }

    public void ResetPlayback()
    {
        csvFrameIndex = 0;
        playbackStartTime = Time.timeAsDouble;
        firstFrame = true;
        if (csvFrames.Count > 0)
            Array.Copy(csvFrames[0], previousFrameValues, Math.Min(99, csvFrames[0].Length));
    }

    // CSV format: header "timestamp,n0,...,n98"
    public bool LoadCsv(string path)
    {
        try
        {
            csvFrames.Clear();
            csvTimes.Clear();
            csvFrameIndex = 0;
            csvDuration = 0.0;
            csvPath = path;

            DateTimeOffset? baseTs = null;

            using (var sr = new StreamReader(path))
            {
                string header = sr.ReadLine(); // skip header
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');
                    if (parts.Length < 100) continue; // 1 timestamp + 99 values

                    var frame = new float[99];
                    bool ok = true;
                    for (int i = 0; i < 99; i++)
                    {
                        if (float.TryParse(parts[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out float v))
                            frame[i] = Mathf.Clamp01(v);
                        else { ok = false; break; }
                    }
                    if (!ok) continue;

                    double tSec;
                    if (useCsvTimestamps &&
                        DateTimeOffset.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
                    {
                        if (!baseTs.HasValue) baseTs = dto;
                        tSec = (dto - baseTs.Value).TotalSeconds;
                    }
                    else
                    {
                        useCsvTimestamps = false;
                        tSec = csvFrames.Count / Mathf.Max(1e-3f, playbackFpsFallback);
                    }

                    csvFrames.Add(frame);
                    csvTimes.Add(tSec);
                }
            }

            if (csvFrames.Count == 0)
            {
                Debug.LogWarning($"[Heatmap] No valid frames in CSV: {path}");
                return false;
            }

            for (int i = 1; i < csvTimes.Count; i++)
                if (csvTimes[i] < csvTimes[i - 1]) csvTimes[i] = csvTimes[i - 1];
            csvDuration = csvTimes[csvTimes.Count - 1];

            Array.Copy(csvFrames[0], previousFrameValues, Math.Min(99, csvFrames[0].Length));
            firstFrame = true;

            Debug.Log($"[Heatmap] Loaded {csvFrames.Count} frame(s) from CSV: {path}. Timing={(useCsvTimestamps ? "timestamps" : $"FPS {playbackFpsFallback}")}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Heatmap] Failed to load CSV '{path}': {ex.Message}");
            return false;
        }
    }

    public bool LoadLatestCsvFrom(string directory)
    {
        try
        {
            if (!Directory.Exists(directory))
            {
                Debug.LogWarning("[Heatmap] Directory not found: " + directory);
                return false;
            }
            var files = Directory.GetFiles(directory, "*.csv");
            if (files.Length == 0)
            {
                Debug.LogWarning("[Heatmap] No CSV files in: " + directory);
                return false;
            }

            string latest = files[0];
            DateTime latestWrite = File.GetLastWriteTimeUtc(latest);
            foreach (var f in files)
            {
                DateTime w = File.GetLastWriteTimeUtc(f);
                if (w > latestWrite) { latest = f; latestWrite = w; }
            }

            bool ok = LoadCsv(latest);
            if (ok) ResetPlayback();
            return ok;
        }
        catch (Exception ex)
        {
            Debug.LogError("[Heatmap] LoadLatestCsvFrom error: " + ex.Message);
            return false;
        }
    }

    public void FixPosition()
    {
        if (heatmap == null) return;
        var handGrab = heatmap.GetComponent<HandGrabInteractable>();
        if (handGrab != null) handGrab.enabled = false;

        var grabbable = heatmap.GetComponent<Grabbable>();
        if (grabbable != null) grabbable.enabled = false;

        var grabInteractable = heatmap.GetComponent<GrabInteractable>();
        if (grabInteractable != null) grabInteractable.enabled = false;
    }

    public void DeactivateManager()
    {
        gameObject.SetActive(false);
    }
}