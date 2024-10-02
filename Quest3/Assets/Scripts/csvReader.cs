using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class csvReader : MonoBehaviour
{
    //public TextAsset expertTrail;

    // Start is called before the first frame update
    void Start()
    {


        List<float> data = srdCSVFile();
        foreach (float value in data)
        {
            Debug.Log(value);
        }
    }


    

    public static List<float> srdCSVFile()
    {
        List<float> records = new List<float>();

        

        string sFilePath = Path.Combine(Application.streamingAssetsPath, "expertTrial_short.csv");
        
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                TextAsset csvFile = Resources.Load<TextAsset>("expertTrial_short");
string[] csvLines = csvFile.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

records = new List<float>();

// Skip the header line
for (int i = 1; i < csvLines.Length; i++)
{
    string line = csvLines[i].Trim();
    if (!string.IsNullOrEmpty(line))
    {
        float value = float.Parse(line);
        records.Add(value);
    }
}
            }
            catch (IOException ex)
            {
                Debug.LogError($"Error reading file: {ex.Message}");
            }
            catch (FormatException ex)
            {
                Debug.LogError($"Error parsing CSV data: {ex.Message}");
            };
        }
        else 


        try
        {
            
            ///OLD Version reading csv///

            string filePath = Application.streamingAssetsPath + "/expertTrial.csv";
            Debug.Log("loaded file: " + filePath);
            //Debug.Log(Application.persistentDataPath);
            //using (StreamReader reader = new StreamReader("Assets/Trials/expertTrial_short.csv"))
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                reader.ReadLine(); // Skip the header line

                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        float value = float.Parse(line);
                        records.Add(value);
                    }
                }
            }

            /////////////////////////
        }
        catch (IOException ex)
        {
            Debug.LogError($"Error reading file: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Debug.LogError($"Error parsing CSV data: {ex.Message}");
        }

        return records;
    }

    internal static List<float> srdCSVFile(string filename)
    {
        throw new NotImplementedException();
    }
}
