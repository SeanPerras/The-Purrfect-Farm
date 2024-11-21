using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    public Farm farm;
    public List<GameObject> catsulePrefabs;
    public GameObject catPrefab;
    public TextMeshProUGUI catCoinsDisplay;
    private Scene scene;
    private int catCoins;
    // Start is called before the first frame update
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
    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        catCoinsDisplay.text = catCoins.ToString();
    }
    private void OnApplicationQuit()
    {
        SaveJsonData(instance);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.Equals(scene.path, "Assets/Scenes/Home.unity")) return;
        
    
        farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
        catCoinsDisplay = GameObject.Find("Currency").GetComponent<TextMeshProUGUI>();
        LoadJsonData(instance);

        if (scene.name == "Home")
    {
        var expeditionsButton = GameObject.Find("Click for Expeditions").GetComponent<Button>();
        if (expeditionsButton != null)
        {
            
            var nightToDay = GetComponent<NightToDay>();
            if (nightToDay != null)
            {
                expeditionsButton.onClick.RemoveAllListeners();
                expeditionsButton.onClick.AddListener(nightToDay.ToExpeditions);
            }
        }
    }

        else if (scene.name == "Expedition Map")
        {
        NightToDay nightToDay = GetComponent<NightToDay>();
        if (nightToDay != null)
        {
            var homeButton = GameObject.Find("Return to Farm").GetComponent<Button>();
            if (homeButton != null)
            {
                homeButton.onClick.RemoveAllListeners();
                homeButton.onClick.AddListener(nightToDay.BackToFarm); 
            }
        }
    }
    }

    
    public void AddCoin(int coin)
    {
        catCoins += coin;
    }
    public int GetCurrency()
    {
        return catCoins;
    }
    public void SetCurrency(int coins)
    {
        catCoins = coins;
    }




    public void PopulateSaveData(SaveData jData)
    {
        jData.currency = GetCurrency();
        SaveData.ImportantPlotInfo plot = new();
        SaveData.ImportantPlantInfo pocl = new();
        foreach (Plot pt in GameObject.Find("Plots").GetComponentsInChildren<Plot>().ToList())
        {
            if (pt.HasPlant())
            {
                pocl.instance = pt.HasPlant();
                Plant plant = pt.GetPlant();
                pocl.plantOrcatsule = "Plant";
                pocl.nameData = plant.plantData.name;
                pocl.growthStage = plant.GrowthStage;
                pocl.timeLeft = plant.Timer;
            }
            else if (pt.HasCatsule())
            {
                pocl.instance = pt.HasCatsule();
                Catsule catsule = pt.GetCatsule();
                pocl.plantOrcatsule = "Catsule";
                pocl.nameData = catsule.name;
                pocl.growthStage = 0;
                pocl.timeLeft = catsule.Timer;
            }
            else
            {
                pocl.instance = false;
                pocl.plantOrcatsule = "";
                pocl.nameData = null;
                pocl.growthStage = 0;
                pocl.timeLeft = 0;
            }

            plot.position = new float[] { pt.transform.position.x, pt.transform.position.y };
            plot.plantOrcatsuleInfo = pocl;
            jData.plots.Add(plot);
        }
        foreach (Cat ct in GameObject.Find("Cats").GetComponentsInChildren<Cat>().ToList())
        {
            jData.cats.Add(ct.stats.name);
        }
    }
    public void LoadFromSaveData(SaveData jData)
    {
        SetCurrency(jData.currency);
        foreach (SaveData.ImportantPlotInfo plot in jData.plots)
        {
            Vector2 pos = new(plot.position[0], plot.position[1]);
            GameObject land = Physics2D.OverlapCircleAll(pos, 0).Select(c => c.gameObject).ToList()
                .Find(c => c.CompareTag("Land"));
            Plot newPlot = farm.Plow(land).GetComponent<Plot>();
            if (plot.plantOrcatsuleInfo.instance && plot.plantOrcatsuleInfo.plantOrcatsule == "Plant")
            {
                PlantData pd = Resources.Load<PlantData>("PlantData/" + plot.plantOrcatsuleInfo.nameData);
                newPlot.Plant(pd);
                Plant pt = newPlot.GetPlant();
                pt.GrowthStage = plot.plantOrcatsuleInfo.growthStage;
                pt.SetPlantData(pd);
                if (pt.GrowthStage >= 3)
                {
                    if (pd.name.Contains("Pepper"))
                        pt.color = pd.growthStages[pt.GrowthStage].name.Split("_")[1];
                    pt.Finished();
                }
                pt.Timer = plot.plantOrcatsuleInfo.timeLeft;
            }
            if (plot.plantOrcatsuleInfo.instance && plot.plantOrcatsuleInfo.plantOrcatsule == "Catsule")
            {
                string color = plot.plantOrcatsuleInfo.nameData.Split(" ")[0];
                GameObject catsule = Instantiate(catsulePrefabs.Find(c => c.GetComponent<Catsule>().color == color),
                    newPlot.transform.position, newPlot.transform.rotation);
                newPlot.GetComponent<Plot>().Plant(catsule.GetComponent<Catsule>());
                newPlot.GetCatsule().GetComponent<Catsule>().Timer = plot.plantOrcatsuleInfo.timeLeft;
            }
        }
        foreach (string cat in jData.cats)
        {
            CatStats cd = Resources.Load<CatStats>("CatData/" + cat);
            GameObject ct = Instantiate(catPrefab, transform.position, catPrefab.transform.rotation);
            ct.GetComponent<Cat>().SetColor(cat[..cat.IndexOf("Cat")]);
        }
    }
    public static void SaveJsonData(GameManager jManager)
    {
        SaveData saveData = new();
        jManager.PopulateSaveData(saveData);
        if (WriteToFile("SaveData.dat", saveData.ToJson()))
            Debug.Log("Save successful!");
    }
    public static void LoadJsonData(GameManager jManager)
    {
        if(LoadFromFile("SaveData.dat", out var sr))
        {
            SaveData saveData = new();
            saveData.LoadFromJson(sr);
            jManager.LoadFromSaveData(saveData);
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
