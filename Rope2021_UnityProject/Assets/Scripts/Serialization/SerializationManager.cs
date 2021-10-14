using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using System.IO;

public class SerializationManager
{
    static string saveExtension = "sav";

    // Start is called before the first frame update
    public static bool Save(string saveName, string savesPath, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(savesPath))
        {
            Directory.CreateDirectory(savesPath);
        }

        string path = savesPath + "/" + saveName + "." + saveExtension;

        FileStream file = File.Create(path);

        formatter.Serialize(file, saveData);

        file.Close();

        return true;
    }


    public static object Load(string saveName, string path)
    {
        string fullPath = path + "/" + saveName + "." + saveExtension;

        if (!File.Exists(fullPath))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(fullPath, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to laod file at {0}", fullPath);
            return null;
        }
    }


    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3SerializationSurrogate = new Vector3SerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SerializationSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
