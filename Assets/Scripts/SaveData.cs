using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Serializable]
    public struct ImportantPlantInfo
    {
        public bool instance;
        public string plantOrcatsule;
        public string nameData;
        public int growthStage;
        public float timeLeft;
    }
    [Serializable]
    public struct ImportantPlotInfo
    {
        public float[] position;
        //public ImportantPlantInfo plantInfo;
        //public ImportantPlantInfo catsuleInfo;
        public ImportantPlantInfo plantOrcatsuleInfo;
    }
    //[Serializable]
    //public struct ImportantCatInfo
    //{
    //    public string nameData;
    //}
    public int currency;
    public List<string> cats = new();
    public List<ImportantPlotInfo> plots = new();
    //public List<string> inventory = new();

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string load)
    {
        JsonUtility.FromJsonOverwrite(load, this);
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData jData);
    void LoadFromSaveData(SaveData jData);
}
