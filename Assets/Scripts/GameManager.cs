using System;
using System.Collections;
using System.Collections.Generic;
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
    public int ExpeditionCoins = 0;
    private bool coinsAdded = false;
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
        if(catCoinsDisplay) catCoinsDisplay.text = catCoins.ToString();
    }
    private void OnApplicationQuit()
    {
        if(SceneManager.GetActiveScene().name == "Home") SaveJsonData(instance);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.path == "Assets/Scenes/Home.unity")
        {
            farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
            catCoinsDisplay = GameObject.Find("Currency").GetComponent<TextMeshProUGUI>();
            LoadJsonData(instance);
            if (GameObject.Find("Click for Expeditions").TryGetComponent(out Button expeditionsButton))
                if (TryGetComponent<NightToDay>(out var nightToDay))
                {
                    expeditionsButton.onClick.RemoveAllListeners();
                    expeditionsButton.onClick.AddListener(ToExpeditions);
                }
        }
        else if (scene.name == "Expedition Map")
        {
            if (TryGetComponent<NightToDay>(out var nightToDay))
                if (GameObject.Find("Return to Farm").TryGetComponent(out Button homeButton))
                {
                    homeButton.onClick.RemoveAllListeners();
                    homeButton.onClick.AddListener(BackToFarm);
                }
        }
    }
    public void BackToFarm()
    {
        Expedition[] allExpeditions = FindObjectsOfType<Expedition>();
        if (ExpeditionSLManager.instance != null)
            ExpeditionSLManager.SaveExpoJsonData(ExpeditionSLManager.instance, allExpeditions.ToList());
        SceneManager.LoadScene("Home");

    }
    public void ToExpeditions()
    {
        if (GameManager.instance != null)
        {
            GameManager.SaveJsonData(GameManager.instance);
            //ExpeditionSLManager.LoadExpoJsonData(ExpeditionSLManager.instance);
            SceneManager.LoadScene("Expedition Map");
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

    public void ExpeditionAddCoin(int coin){
        ExpeditionCoins = coin;
    }




    public void PopulateSaveData(SaveData jData)
    {
        jData.currency = GetCurrency();
        foreach (Plot pt in GameObject.Find("Plots").GetComponentsInChildren<Plot>().ToList())
        {
            bool PLANT = pt.HasPlant(), CATSULE = pt.HasCatsule();
            Plant plant = new(); Catsule catsule = new();
            if (PLANT) plant = pt.GetPlant();
            else if (CATSULE) catsule = pt.GetCatsule();

            SaveData.ImportantPlantInfo pocl = new()
            {
                instance = PLANT ? true : CATSULE,
                plantOrcatsule = PLANT ? "Plant" : CATSULE ? "Catsule" : "",
                nameData = PLANT ? plant.plantData.name : CATSULE ? catsule.name : "",
                growthStage = PLANT ? plant.GrowthStage : 0,
                timeLeft = PLANT ? plant.Timer : CATSULE ? catsule.Timer : 0
            };

            SaveData.ImportantPlotInfo plot = new()
            {
                position = new float[] { pt.transform.position.x, pt.transform.position.y },
                plantOrcatsuleInfo = pocl
            };
            jData.plots.Add(plot);
        }
        foreach (Cat ct in GameObject.Find("Cats").GetComponentsInChildren<Cat>().ToList())
        {
            jData.cats.Add(ct.stats.name);
        }
    }
    public void LoadFromSaveData(SaveData jData)
    {
        //Debug.Log("Loading Save Data...");
        //Debug.Log($"ExpeditionCoins: {ExpeditionCoins}, CoinsAdded: {coinsAdded}");
        if(ExpeditionCoins > 0){
            if(!coinsAdded){
                catCoins += ExpeditionCoins;
                //ExpeditionCoins = 0;
                catCoinsDisplay.text = catCoins.ToString();
                jData.currency = catCoins;
                SetCurrency(jData.currency);
                coinsAdded = true;
                ExpeditionCoins = 0;
            }
            coinsAdded = false;
            }

        else if (coinsAdded){
            //Debug.Log("Setting currency from saved data: " + jData.currency);
            SetCurrency(jData.currency);
        }
        
        //SetCurrency(jData.currency);
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
                    pt.Water();
                    newPlot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
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
        for(int i = 0; i < jData.cats.Count; i++)
        {
            string cat = jData.cats[i];
            CatStats cd = Resources.Load<CatStats>("CatData/" + cat);
            GameObject cg = Instantiate(catPrefab, transform.position, catPrefab.transform.rotation);
            Cat ct = cg.GetComponent<Cat>();
            cg.transform.position = ct.WithinRange(ct.catioPos);
            cg.GetComponent<SpriteRenderer>().sortingOrder = jData.cats.Count - (i + 1);
            ct.SetColor(cat[..cat.IndexOf("Cat")]);
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
    
    public void PopulateExpoSaveData(ExpoSaveData jData)
    {

        foreach (Expedition expedition in FindObjectsOfType<Expedition>())
        {
            ExpoSaveData.ImportantExpoData expo = new()
            {
                inProgress = expedition.inProgress,
                isCompleted = expedition.isCompleted,
                timeLeft = expedition.expeditionTimer,
                team = expedition.teamMemberNames
            };

            jData.expeditions.Add(expo);
        }
    }
    public void LoadFromExpoSaveData(ExpoSaveData jData)
    {
        Expedition[] expeditions = FindObjectsOfType<Expedition>();
        int expeditionCount = Mathf.Min(jData.expeditions.Count, expeditions.Length);

        List<GameObject> restoredTeam = new();
        for (int i = 0; i < expeditionCount; i++)
        {
            ExpoSaveData.ImportantExpoData expo = jData.expeditions[i];
            Expedition currentExpedition = expeditions[i];
            if (expo.timeLeft > 0 || expo.isCompleted)
            {
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
    public static void SaveExpoJsonData(GameManager jManager)
    {
        if (jManager == null)
        {
            Debug.LogError("SaveExpoJsonData: jManager is null.");
            return;
        }
        ExpoSaveData saveData = new();
        jManager.PopulateExpoSaveData(saveData);
        if (WriteToFile("ExpoSaveData.dat", saveData.ToJson()))
            Debug.Log("Save successful!");
    }
    public static void LoadExpoJsonData(GameManager jManager)
    {
        if (LoadFromFile("ExpoSaveData.dat", out var sr))
        {
            ExpoSaveData saveData = new();
            saveData.LoadFromJson(sr);
            jManager.LoadFromExpoSaveData(saveData);
            Debug.Log("Load successful!");
        }
    }
}
