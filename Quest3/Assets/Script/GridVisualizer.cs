using UnityEngine;
public class GridVisualizer : MonoBehaviour
{
    public GameObject cubePrefab;
    public SerialReader serialReader;
    public float spacing = 0.2f; // Space between cubes
    public GameObject[,] cubes = new GameObject[4, 4];
    [Tooltip("Fill this from SerialReader to visualize sensor values.")]
    public int[] sensorValues = new int[16];
    void Start()
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                Vector3 position = new Vector3(col * spacing, 0, -row * spacing);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, transform);
                cube.name = $"Cube_{row}_{col}";
                cubes[row, col] = cube;
            }
        }
    }
    // void Update()
    // {
    //     if (sensorValues.Length != 16) return;
    //     for (int i = 0; i < 16; i++)
    //     {
    //         int row = i / 4;
    //         int col = i % 4;
    //         GameObject cube = cubes[row, col];
    //         float norm = Mathf.InverseLerp(0, 255, sensorValues[i]); // Normalize sensor to 0–1
    //         float newHeight = Mathf.Lerp(0.1f, 2.0f, norm);           // Height from 0.1 to 2.0
    //         Vector3 scale = cube.transform.localScale;
    //         scale.y = newHeight;
    //         cube.transform.localScale = scale;
    //         // Update color: black (low) to red (high)
    //         Renderer renderer = cube.GetComponent<Renderer>();
    //         if (renderer != null)
    //         {
    //             renderer.material.color = Color.Lerp(Color.black, Color.red, norm);
    //         }
    //     }
    // }

    void Update()
{
    if (serialReader == null || serialReader.sensorValues == null)
    {
        Debug.LogWarning("SerialReader not assigned or sensorValues null.");
        return;
    }
    Debug.Log("Updating GridVisualizer..."); // <--- Add this
    for (int row = 0; row < 4; row++)
    {
        for (int col = 0; col < 4; col++)
        {
            int index = row * 4 + col;
            float value = Mathf.InverseLerp(2100, 2800, serialReader.sensorValues[index]); // Normalize
            GameObject cube = cubes[row, col];
            // Scale height
            Vector3 scale = cube.transform.localScale;
            scale.y = Mathf.Lerp(0.1f, 2f, value);
            cube.transform.localScale = scale;
            // Color
            Color color = Color.Lerp(Color.black, Color.red, value);
            cube.GetComponent<Renderer>().material.color = color;
        }
    }
}


}

