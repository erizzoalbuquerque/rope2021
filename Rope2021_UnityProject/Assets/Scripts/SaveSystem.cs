using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public BlockBuilder blockBuilder;
    public string saveName;

    string savePath;

    private void Awake()
    {
        savePath = Application.dataPath + "/saves";
    }

    [ContextMenu("Save")]
    public bool Save()
    {
        blockBuilder.Save();
        SerializationManager.Save(saveName, savePath, SaveData.Instance);

        return true;
    }

    public bool Save(string fileName)
    {
        this.saveName = fileName;
        return Save();
    }


    [ContextMenu("Load")]
    public bool Load()
    {
        object result = SerializationManager.Load(saveName, savePath);
        if (result == null)
            return false;

        SaveData.Instance = (SaveData) result;        

        blockBuilder.Load();

        return true;
    }


    public bool Load(string fileName)
    {
        this.saveName = fileName;
        return Load();
    }
}
