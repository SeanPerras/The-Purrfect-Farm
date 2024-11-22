using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ExpoSaveData
{


    [Serializable]
    public struct ImportantExpoData{
        public bool inProgress;
        public bool isCompleted;
        public float timeLeft;
        public List<string> team;
    }


    public List<ImportantExpoData> expeditions = new();


    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string load)
    {
        JsonUtility.FromJsonOverwrite(load, this);
    }
   
}

public interface IExpoSaveable
{
    void PopulateExpoSaveData(ExpoSaveData jData, List<Expedition> expeditions);
    void LoadFromExpoSaveData(ExpoSaveData jData);
}
