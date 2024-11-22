using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ExpeditionSLManager : MonoBehaviour, IExpoSaveable
{

    public static ExpeditionSLManager instance;
    //public List<ExpeditionUI> expeditionUIs;

     void Awake()
    {
        if (!instance)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (!instance.gameObject.activeSelf) instance.gameObject.SetActive(true);  // Reactivate the original instance if it's disabled
            // Destroy the duplicate GameManager
            Destroy(gameObject);
            return;
        }

    }

    private void OnApplicationQuit()
    {
        Expedition currentExpedition = FindObjectOfType<Expedition>();
        SaveExpoJsonData(instance, currentExpedition);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.Equals(scene.path, "Assets/Scenes/Home.unity")) return;
        
    
        LoadExpoJsonData(instance);
    }


    public void PopulateExpoSaveData(ExpoSaveData jData, Expedition expedition)
    {
        ExpoSaveData.ImportantExpoData expo = new();
           
        expo.inProgress = expedition.inProgress;
        expo.isCompleted = expedition.isCompleted;
        expo.timeLeft = expedition.expeditionTimer;
        expo.team = expedition.teamMemberNames;
            
        jData.expeditions.Add(expo);
        
    }
    public void LoadFromExpoSaveData(ExpoSaveData jData)
    {
    
    }

    
    public static void SaveExpoJsonData(ExpeditionSLManager jManager, Expedition currentExpedition)
    {
        if (jManager == null)
    {
        Debug.LogError("SaveExpoJsonData: jManager is null.");
        return;
    }
        ExpoSaveData saveData = new();
        jManager.PopulateExpoSaveData(saveData, currentExpedition);
        if (WriteToFile("ExpoSaveData.dat", saveData.ToJson()))
            Debug.Log("Save successful!");
    }
    public static void LoadExpoJsonData(ExpeditionSLManager jManager)
    {
        if(LoadFromFile("ExpoSaveData.dat", out var sr))
        {
            ExpoSaveData saveData = new();
            saveData.LoadFromJson(sr);
            jManager.LoadFromExpoSaveData(saveData);
            Debug.Log("Load successful!");
        }
    }
    public static bool WriteToFile(string fileName, string saveData)
    {
        var fPath = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            File.WriteAllText(fPath, saveData);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fPath} with exception {e}");
            return false;
        }
    }
    public static bool LoadFromFile(string fileName, out string data)
    {
        var fPath = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            data = File.ReadAllText(fPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fPath} with exception {e}");
            data = "";
            return false;
        }
    }

}