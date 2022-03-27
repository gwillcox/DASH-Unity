using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LoggerSettings 
{
    public string dataName;
    public int maxMeasurements;
}
 
[Serializable]
public class DataLogger
{
    LoggerSettings settings = new LoggerSettings();
    List<float> data = new List<float>();

    // Start is called before the first frame update
    public DataLogger(string name = "unnamed")
    { 
        settings.dataName = name;
    }

    public void Log(float f) 
    { 
        data.Add(f); 
    }

    public void Clear() { data.Clear(); }

    public void SaveToFile(string filename) 
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(filename, json);
    }
}

