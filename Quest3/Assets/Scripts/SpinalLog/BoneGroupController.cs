using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;
using System;
using System.IO;
public class BoneGroupController : MonoBehaviour
{
    public GameObject boneL2;
    public GameObject boneL3;
    public GameObject boneL4;
    public GameObject boneL5;

    [SerializeField]
    private SpinalLogBluetoothManager BTManager;
    private bool firstConnect = true;

    private GameObject[] boneGroup;
    private GameObject focusBone;
    // Start is called before the first frame update

    // Logging
    [Header("Logging")]
    [SerializeField] private bool enableLogging = true;
    [SerializeField] private float logAngleThreshold = 0.5f; // degrees to consider "movement"
    [SerializeField] private float flushInterval = 2f;       // seconds
    private float lastFlushTime = 0f;
    private StreamWriter logWriter;
    private readonly Dictionary<GameObject, Quaternion> lastRotations = new Dictionary<GameObject, Quaternion>();

    void Start()
    {
        boneGroup = new GameObject[] { boneL2, boneL3, boneL4, boneL5 };
        if (enableLogging)
        {
            InitLogger();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BTManager.BTHelper != null)
        {
            // update depth of each sensor, have to store initial distance with no pressure
            if (BTManager.BTHelper != null && firstConnect)
            {
                //UnityDebug.Log("11111");
                SetInitialBoneDepth(BTManager.distances);
                firstConnect = false;
            }
            else
            {
                SetCurBoneDepth(BTManager.distances);
            }
            focusBone = FindFocusBoneDepth();
            //UnityDebug.Log("focusBone: " + focusBone.GetComponent<BoneController>().boneID);

            // count rotation degree
            foreach (GameObject bone in boneGroup)
            {
                var bc = bone.GetComponent<BoneController>();

                bc.Rotation(focusBone.GetComponent<BoneController>().averageDepth, focusBone.GetComponent<BoneController>().boneID);

                // Logging on movement
                if (enableLogging && logWriter != null)
                {
                    Quaternion current = bone.transform.localRotation;

                    if (!lastRotations.TryGetValue(bone, out var prev))
                    {
                        lastRotations[bone] = current; // seed without logging
                    }
                    else
                    {
                        float delta = Quaternion.Angle(prev, current);
                        if (delta >= logAngleThreshold)
                        {
                            Vector3 e = bone.transform.localEulerAngles;
                            int focusId = focusBone != null ? focusBone.GetComponent<BoneController>().boneID : -1;

                            logWriter.WriteLine($"{DateTime.UtcNow:o},{bc.boneID},{bc.averageDepth:F3},{e.x:F2},{e.y:F2},{e.z:F2},{focusId}");
                            lastRotations[bone] = current;
                        }
                    }
                }
            }

            // Periodic flush
            if (enableLogging && logWriter != null && Time.unscaledTime - lastFlushTime > flushInterval)
            {
                logWriter.Flush();
                lastFlushTime = Time.unscaledTime;
            }

        }

    }

    public void SetInitialBoneDepth(float[] depths)
    {
        for (int i = 0; i < boneGroup.Length; i++)
        {
            boneGroup[i].GetComponent<BoneController>().SetInitialDepth(depths[i * 2], depths[i * 2 + 1]);
        }
    }

    public void SetCurBoneDepth(float[] depths)
    {
        for (int i = 0; i < boneGroup.Length; i++)
        {
            boneGroup[i].GetComponent<BoneController>().SetCurDepth(depths[i * 2], depths[i * 2 + 1]);
        }
    }

    GameObject FindFocusBoneDepth()
    {
        float smallestDepth = 0;
        GameObject target = null;
        for (int i = 0; i < boneGroup.Length; i++)
        {
            float depth = boneGroup[i].GetComponent<BoneController>().averageDepth;
            if (smallestDepth == 0 || smallestDepth > depth)
            {
                smallestDepth = depth;
                target = boneGroup[i];
            }
        }

        return target;
    }
    
    private void InitLogger()
    {
        try
        {
            string dir = Application.persistentDataPath;
            string path = Path.Combine(dir, $"bone-movements_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            logWriter = new StreamWriter(path, false);
            logWriter.WriteLine("timestampUTC,boneID,avgDepth,rotX,rotY,rotZ,focusBoneID");
            logWriter.Flush();
            UnityDebug.Log($"Bone movement logs -> {path}");
        }
        catch (Exception ex)
        {
            UnityDebug.LogError($"Failed to init logger: {ex.Message}");
            enableLogging = false;
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (logWriter != null)
            {
                logWriter.Flush();
                logWriter.Dispose();
                logWriter = null;
            }
        }
        catch {}
    }
}
