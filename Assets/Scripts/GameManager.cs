using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct InventoryStruct
{
    //public InventoryStruct()
    //{
    //    seeds = new();
    //    catsules = new();
    //    decors = new();
    //}
    public Dictionary<string, int> seeds, catsules, decors;
    public readonly bool TryGet(string item, out int ret)
    {
        if (seeds.TryGetValue(item, out ret)) return true;
        else if (catsules.TryGetValue(item, out ret)) return true;
        else if (decors.TryGetValue(item, out ret)) return true;
        else return false;
    }
    public readonly bool Contains(string item)
    {
        if (seeds.ContainsKey(item)) return true;
        else if (catsules.ContainsKey(item)) return true;
        else if (decors.ContainsKey(item)) return true;
        else return false;
    }
    public readonly int this[string key]
    {
        get
        {
            if (seeds.TryGetValue(key, out int v)) return v;
            else if (catsules.TryGetValue(key, out v)) return v;
            else if (decors.TryGetValue(key, out v)) return v;
            else throw new KeyNotFoundException("Item not found in Inventory");
        }
        set
        {
            if (seeds.ContainsKey(key)) seeds[key] = value;
            else if (catsules.ContainsKey(key)) catsules[key] = value;
            else if (decors.ContainsKey(key)) decors[key] = value;
            else throw new KeyNotFoundException("Item not found in Inventory");
        }
    }
    public readonly void RemoveZeroes()
    {
        foreach (var seed in seeds.Where(s => s.Value == 0).ToList()) seeds.Remove(seed.Key);
        foreach (var catsule in catsules.Where(c => c.Value == 0).ToList()) catsules.Remove(catsule.Key);
        foreach (var decor in decors.Where(d => d.Value == 0).ToList()) decors.Remove(decor.Key);
    }
}
public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    public Farm farm;//Farm (9x9)
    public List<GameObject> catsulePrefabs;
    public GameObject catPrefab, FARM, EXPO;
    public Slider musicVolume, sfxVolume;
    public TextMeshProUGUI catCoinsDisplay;
    public InventoryStruct Inventory = new() { seeds = new(), catsules = new(), decors = new() };
    public List<Coroutine> currentCoroutines = new();

    private readonly static string defaultSave =
        "{\"inventory\":{\"seeds\":[\"Blueberry:5\"],\"catsules\":[\"White:1\"],\"decors\":[]},\"currency\":45,\"cats\":[],\"plots\":[]}";
    private int catCoins;
    private bool confirmed = false, quickSell = false;
    private GameObject confirmedObj;
    void Awake()
    {
        if (!instance)
        {
            instance = this;
            //SceneManager.sceneLoaded += OnSceneLoaded;
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
        LoadJsonData(instance);
    }

    // Update is called once per frame
    void Update()
    {
        if(catCoinsDisplay && int.Parse(catCoinsDisplay.text) != catCoins)
            catCoinsDisplay.text = catCoins.ToString();
    }
    private void OnApplicationQuit()
    {
        //if (FARM.activeSelf) SaveJsonData(instance);
        //if (EXPO.activeSelf) SaveExpoJsonData(instance);
    }
    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
    /*private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Home")
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
    }*/
    public void BackToFarm()
    {
        if (instance != null)
        {
            //SaveExpoJsonData(instance);
            foreach (Transform button in ExpeditionManager.instance.catButtons) Destroy(button.gameObject);
            EXPO.SetActive(false);
            FARM.SetActive(true);
            farm.PopulateButtons(Inventory);
        }

    }
    public void ToExpeditions()
    {
        if (instance != null)
        {
            //SaveJsonData(instance);
            foreach (Transform button in farm.seedSelectParent) Destroy(button.gameObject);
            foreach (Transform button in farm.catsuleSelectParent) Destroy(button.gameObject);
            foreach (Transform button in farm.inventoryParent) Destroy(button.gameObject);
            Inventory.RemoveZeroes();
            if (farm.inventoryParent.childCount < 5)
                farm.inventoryParent.GetComponent<RectTransform>().sizeDelta = new(1550, 300);
            List<Cat> cats = GameObject.Find("Cats").GetComponentsInChildren<Cat>().ToList();
            FARM.SetActive(false);
            EXPO.SetActive(true);
            ExpeditionManager.instance.PopulateCats(cats);
        }
    }
    public void SaveGame()
    {
        SaveJsonData(instance);
        SaveExpoJsonData(instance);
    }
    public IEnumerator ResetSave()
    {
        yield return new WaitUntil(() => confirmed);
        confirmed = false;
        foreach (Transform tf in farm.catsParent.transform) Destroy(tf.gameObject);
        foreach (Transform tf in farm.plotsParent.transform) tf.GetComponent<Plot>().Sell();
        foreach (Transform tf in farm.seedSelectParent) Destroy(tf.gameObject);
        foreach (Transform tf in farm.catsuleSelectParent) Destroy(tf.gameObject);
        SaveData saveData = new(defaultSave);
        if (WriteToFile("SaveData.dat", saveData.ToJson()))
            instance.LoadFromSaveData(saveData);
        farm.PopulateButtons(instance.Inventory);
    }
    public void ObjectToConfirm(GameObject obj) { confirmedObj = obj; }
    public void WaitForConfirmation(string method)
    {
        confirmed = false;
        if (confirmedObj.TryGetComponent(out Plot pl))
            pl.Sell();
        else if (method.Contains('.'))
        {
            farm.confirmMenu.SetActive(true);
            string script = method[..method.IndexOf(".")];
            Type type = Type.GetType(script);
            currentCoroutines.Add((confirmedObj.GetComponent(type) as MonoBehaviour).StartCoroutine(method[(method.IndexOf(".") + 1)..]));
        }
        else
        {
            farm.confirmMenu.SetActive(true);
            currentCoroutines.Add(StartCoroutine(method));
        }
    }
    public void Confirm()
    {
        confirmed = true;
        StartCoroutine(Farm.DelayMenu(farm.confirmMenu));
    }
    public void Cancel()
    {
        confirmed = false;
        //StopCoroutine(currentCoroutine);
        //StopAllCoroutines();
        foreach(Coroutine cr in new List<Coroutine>(currentCoroutines))
        {
            StopCoroutine(cr);
            currentCoroutines.Remove(cr);
        }
        StartCoroutine(Farm.DelayMenu(farm.confirmMenu));
    }
    public bool IsConfirmed() { return confirmed; }
    public void ToggleQuickSell(Toggle toggle) { quickSell = toggle.isOn; }
    public bool IsQuickSell() { return quickSell; }

    public void AddCoin(int coin) { catCoins += coin; }
    public void RemoveCoin(int coin) { catCoins -= coin; }
    public int GetCurrency() { return catCoins; }
    public void SetCurrency(int coins) { catCoins = coins; }
    public void UpdateInventory(string item, string type)
    {
        //if(Inventory.Find(item, out _))
        //{
        //    Inventory[item]++
        //    nb = false;
        //}
        //else
        //{
        //    Inventory[item] = 1;
        //    nb = true;
        //}
        //if (Inventory.Contains(item))
        //    Inventory[item] += 1;
        //else Inventory[item] = 1;
        //farm.UpdateButtons();//--------------------------------------------------
        if (type == "Seed")
        {
            if (Inventory.seeds.ContainsKey(item))
                Inventory.seeds[item]++;
            else
                Inventory.seeds[item] = 1;
            farm.UpdateButtons(farm.seedSelectParent, item);
        }
        else if (type == "Catsule")
        {
            if (Inventory.catsules.ContainsKey(item))
                Inventory.catsules[item]++;
            else
                Inventory.catsules[item] = 1;
            farm.UpdateButtons(farm.catsuleSelectParent, item);
        }
        else
        {
            farm.UpdateIcons(item);
        }
    }
    public bool IsAUIOpen()
    {
        return farm.IsAnyUIOpen();
    }




    public void PopulateSaveData(SaveData jData)
    {
        PlayerPrefs.SetFloat("Music", musicVolume.value);
        PlayerPrefs.SetFloat("SFX", sfxVolume.value);
        PlayerPrefs.Save();
        jData.currency = GetCurrency();
        foreach (Plot pt in GameObject.Find("Plots").GetComponentsInChildren<Plot>().ToList())
        {
            bool PLANT = pt.HasPlant(), CATSULE = pt.HasCatsule();
            Plant plant = null; Catsule catsule = null;
            if (PLANT) plant = pt.GetPlant();
            else if (CATSULE) catsule = pt.GetCatsule();

            SaveData.ImportantPlantInfo pocl = new()
            {
                instance = PLANT || CATSULE,
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
            jData.cats.Add(ct.stats.name);
        
        jData.inventory.seeds = Inventory.seeds
            .Where(kvp => kvp.Value != 0).Select(kvp => kvp.Key + ":" + kvp.Value.ToString()).ToList();
        jData.inventory.catsules = Inventory.catsules
            .Where(kvp => kvp.Value != 0).Select(kvp => kvp.Key + ":" + kvp.Value.ToString()).ToList();
        jData.inventory.decors = Inventory.decors
            .Where(kvp => kvp.Value != 0).Select(kvp => kvp.Key + ":" + kvp.Value.ToString()).ToList();
    }
    public void LoadFromSaveData(SaveData jData)
    {
        //Debug.Log("Loading Save Data...");
        //Debug.Log($"ExpeditionCoins: {ExpeditionCoins}, CoinsAdded: {coinsAdded}");
        //if (ExpeditionCoins > 0)
        //{
        //    jData.currency += ExpeditionCoins;
        //    ExpeditionCoins = 0;
        //}
        musicVolume.value = PlayerPrefs.GetFloat("Music", 1);
        sfxVolume.value = PlayerPrefs.GetFloat("SFX", 1);
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
                Plant pt = newPlot.Plant(pd);
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
                Catsule c = newPlot.GetComponent<Plot>().Plant(color);
                c.Timer = plot.plantOrcatsuleInfo.timeLeft;
            }
        }
        for(int i = 0; i < jData.cats.Count; i++)
        {
            string cat = jData.cats[i];
            //CatStats cd = Resources.Load<CatStats>("CatData/" + cat);
            //GameObject cg = Instantiate(catPrefab, transform.position, catPrefab.transform.rotation);
            Cat ct = Instantiate(catPrefab, transform.position, catPrefab.transform.rotation).GetComponent<Cat>();
            ct.SetColor(cat[..cat.IndexOf("Cat")]);
        }
        Inventory.seeds = jData.inventory.seeds.ToDictionary(s => s[..s.IndexOf(":")], s => int.Parse(s[(s.IndexOf(":") + 1)..]));
        Inventory.catsules = jData.inventory.catsules.ToDictionary(c => c[..c.IndexOf(":")], c => int.Parse(c[(c.IndexOf(":") + 1)..]));
        Inventory.decors = jData.inventory.decors.ToDictionary(d => d[..d.IndexOf(":")], d => int.Parse(d[(d.IndexOf(":") + 1)..]));
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
            //SaveData saveData = new();
            //saveData.LoadFromJson(sr);
            jManager.LoadFromSaveData(new(sr));
            Debug.Log("Load successful!");
        }
        else
        {
            jManager.ResetSave();
            Debug.Log("First Load successful");
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
            if (!data.Contains("inventory"))
                return false;
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
                List<GameObject> temp = new();
                foreach(GameObject c in restoredTeam)
                {
                    currentExpedition.CatFaces(true, c.GetComponent<SpriteRenderer>().sprite);
                    temp.Add(ExpeditionManager.instance.catButtons.Find(c.name).gameObject);
                }
                currentExpedition.SetSelectedTeam(restoredTeam, temp);
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
