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
        List<Expedition> allExpeditions = new List<Expedition>(FindObjectsOfType<Expedition>());
        SaveExpoJsonData(ExpeditionSLManager.instance, allExpeditions);
        //SaveExpoJsonData(instance, currentExpedition);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.Equals(scene.path, "Assets/Scenes/Expedition Map.unity")) return;
        
    
        LoadExpoJsonData(instance);
    }


    public void PopulateExpoSaveData(ExpoSaveData jData, List<Expedition> expeditions)
    {

        foreach (Expedition expedition in expeditions)
    {
        ExpoSaveData.ImportantExpoData expo = new();
           
        expo.inProgress = expedition.inProgress;
        expo.isCompleted = expedition.isCompleted;
        expo.timeLeft = expedition.expeditionTimer;
        expo.team = expedition.teamMemberNames;
            
        jData.expeditions.Add(expo);
    }
        
    }
    public void LoadFromExpoSaveData(ExpoSaveData jData)
    {
        //ExpoSaveData.ImportantExpoData expo = jData.expeditions[0];
        //Expedition currentExpedition = FindObjectOfType<Expedition>();
        //currentExpedition.LoadTimer(expo.timeLeft);

        Expedition[] expeditions = FindObjectsOfType<Expedition>(); 
        int expeditionCount = Mathf.Min(jData.expeditions.Count, expeditions.Length);

        List<GameObject> restoredTeam = new List<GameObject>();
        for(int i = 0; i<expeditionCount; i++){
            ExpoSaveData.ImportantExpoData expo = jData.expeditions[i];
            Expedition currentExpedition = expeditions[i];
            if(expo.timeLeft>0 || expo.isCompleted){
            currentExpedition.LoadTimer(expo.timeLeft, expo.isCompleted);
            restoredTeam.Clear();
         foreach (string name in expo.team)
    {
        Cat cat = CatManager.instance.GetCatByName(name);
        if (cat != null)
        {
            restoredTeam.Add(cat.gameObject);
        }
        else
        {
            Debug.LogWarning($"Cat with name {name} not found!");
        }
    }

    currentExpedition.SetSelectedTeam(restoredTeam);

    }
        }
    
    }

    
    public static void SaveExpoJsonData(ExpeditionSLManager jManager, List<Expedition> expeditions)
    {
        if (jManager == null)
    {
        Debug.LogError("SaveExpoJsonData: jManager is null.");
        return;
    }
        ExpoSaveData saveData = new();
        jManager.PopulateExpoSaveData(saveData, expeditions);
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