using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveData
{
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveData();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public int numberOfBLocks;
    public Vector3[] positions;
}