using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Farm : MonoBehaviour
{
    public static readonly Dictionary<string, List<string>> Colors = new()
    {
        {"Black",  new() {"Black", "Black"} },
        //{"Gray", new() {"Black"} },
        {"Blue", new () {"Blue", "Blue"} },
        {"Sky Blue", new () {"Blue"} },
        {"Red", new() { "Red", "Red" } },
        {"Purple", new() { "Red", "Blue" } },
        {"Pink", new() { "Red" } },
        {"Yellow", new() { "Yellow", "Yellow" } },
        {"Orange", new() { "Red", "Yellow" } }
    };
    public GameObject
        plotPrefab, buttonPF, iconPF, shovel, plotsParent, catsParent,
        seedSelectUI, catSelectUI, catsuleSelectBox, seedSelectBox, wateringCanSelectBox, shovelSelectBox,
        pauseMenu, optionsMenu, shopMenu, inventoryMenu, sellMenu, confirmMenu;//, allMenus;
    public Transform seedSelectParent, catsuleSelectParent, inventoryParent;
    public Texture2D pPointer, pDrag;

    private GameObject plotSelected;
    private List<GameObject> allMenus;
    private bool isWateringMode = false, plowMode = false;
    private Vector3 mousePos;
    private Vector2 offset = new(158, -35), spacing = new(0, -70),
            iOffset = new(125, -140), iSpacing = new(275, 0);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(pPointer, new Vector2(0, pPointer.height*.07f), CursorMode.Auto);
        PopulateButtons(GameManager.Inventory);
        allMenus = new() { seedSelectUI, catSelectUI, pauseMenu, optionsMenu,
            shopMenu, inventoryMenu, sellMenu, confirmMenu };
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero) && !IsAnyUIOpen())
                HandleClick();
            else if (sellMenu.activeSelf)
            {
                sellMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                sellMenu.transform.GetChild(0).gameObject.SetActive(false);
                StartCoroutine(DelayMenu(sellMenu));//.SetActive(false);
            }
        }
        else if (Input.GetMouseButtonDown(1))
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //sellMenu.transform.GetChild(0).gameObject.SetActive(false);
        //sellMenu.SetActive(false);
        //sellMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        else if (Input.GetMouseButton(1))
        {
            if (mousePos != Vector3.zero && Camera.main.ScreenToWorldPoint(Input.mousePosition) != mousePos)
            {
                Cursor.SetCursor(pDrag, new Vector2(pDrag.width * .5f, pDrag.height * .5f), CursorMode.Auto);
                Camera.main.transform.position -= Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;
            }
        }
        else if (Input.GetMouseButtonUp(1))
            Cursor.SetCursor(pPointer, new Vector2(0, pPointer.height * .07f), CursorMode.Auto);
        else if (Input.GetAxis("Mouse ScrollWheel") != 0 && !IsAnyUIOpen())
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * 5, 10, 25);
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 5, 10, 25);
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 5, 10, 25);
        }
        if (isWateringMode && plotsParent.GetComponentsInChildren<Plot>().All(x => !x.IsWaterable()))
        {
            //StartCoroutine(SlowToggle(false, (bool value) => isWateringMode = value, wateringCanSelectBox));
            isWateringMode = false;
            wateringCanSelectBox.GetComponent<Image>().color = Color.white *.5f;
            wateringCanSelectBox.SetActive(false);
        }
        else if(plowMode && transform.GetComponentsInChildren<Transform>().All(go => !go.gameObject.activeSelf))
        {
            plowMode = false;
            shovelSelectBox.GetComponent<Image>().color = Color.white * .5f;
            shovelSelectBox.SetActive(false);
        }
    }

    private void HandleClick()
    {
        //Debug.Log("Mouse Up!");
        Vector3 db = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List<GameObject> collidedGameObjects = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0)
                .Select(c => c.gameObject).ToList();
        GameObject cat = collidedGameObjects.Find(c => c.CompareTag("Cat"));
        GameObject land = collidedGameObjects.Find(l => l.CompareTag("Land"));
        collidedGameObjects.Remove(land);
        GameObject plot = collidedGameObjects.Find(p => p.CompareTag("Plot"));
        collidedGameObjects.Remove(plot);
        GameObject plant = collidedGameObjects.Find(pl => pl.CompareTag("Plant"));
        collidedGameObjects.Remove(plant);
        if (cat) collidedGameObjects.Remove(cat); //Do nothing
        else if ((plant || (plot && plot.GetComponent<Plot>().HasPlant())) && isWateringMode)
            plant.GetComponent<Plant>().Water();
        else if (plant && plant.TryGetComponent(out Plant pc))
            pc.CheckHarvest();
        else if (land && plowMode && !isWateringMode && GameManager.CheckCost(5))
        {
            //shovel.SetActive(true);
            //shovel.transform.position = land.transform.position;
            //Animator anim = shovel.GetComponent<Animator>();
            //anim.Play("Dig Animation");
            GameManager.instance.RemoveCoin(5);
            Plow(land);
            //StartCoroutine(Wait(anim));
        }
        else if (collidedGameObjects.Count == 0 && plot && !isWateringMode && !plowMode)
        {
            plotSelected = plot;
            seedSelectBox.transform.parent.gameObject.SetActive(true);
            catsuleSelectBox.transform.parent.gameObject.SetActive(true);
            OpenSeedUI();
        }
        collidedGameObjects.Clear();
    }
    public GameObject Plow(GameObject landRef)
    {
        Vector3 pos = landRef.transform.position;
        GameObject newPlot = Instantiate(plotPrefab, pos, plotPrefab.transform.rotation, plotsParent.transform);
        newPlot.GetComponent<SpriteRenderer>().sortingOrder = landRef.GetComponent<SpriteRenderer>().sortingOrder;
        newPlot.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = landRef.GetComponent<SpriteRenderer>().sortingOrder;
        newPlot.transform.Find("Outline").GetComponent<SpriteRenderer>().sortingOrder = landRef.GetComponent<SpriteRenderer>().sortingOrder + 1;
        newPlot.GetComponent<Plot>().SetLandRef(landRef);
        //Debug.Log("Plot placed!");
        return newPlot;
    }
    public void Plant(PlantData plantData)
    {
        if (plotSelected)
        {
            plotSelected.GetComponent<Plot>().Plant(plantData);
            plotSelected = null;
            GameManager.Inventory[plantData.plantName]--;
            UpdateIcons(plantData.plantName + " Seed");
        }
        CloseSeedUI();
        if (!catSelectUI.activeSelf) HideIcons();
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
    public void Plant(string color = "White")
    {
        if (plotSelected)
        {
            //GameObject prefab = GameManager.instance.catsulePrefabs.Find(c => c.GetComponent<Catsule>().color == color);
            //GameObject catsule = Instantiate(prefab, plotSelected.transform.position, plotSelected.transform.rotation);
            //plotSelected.GetComponent<Plot>().Plant(catsule.GetComponent<Catsule>());
            plotSelected.GetComponent<Plot>().Plant(color);
            //plotSelected.GetComponent<Collider2D>().enabled = true;
            plotSelected = null;
            GameManager.Inventory[color]--;
        }
        CloseCatUI();
        if (!seedSelectUI.activeSelf) HideIcons();
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
    public void SetSelectedPlot(GameObject plot) { plotSelected = plot; }
    public void CreateMenuButton(string k, int v, Transform parent, int count)
    {
        GameObject newBtn = Instantiate(buttonPF, parent);
        newBtn.transform.localPosition = offset + spacing * count;
        newBtn.name = k + " " + parent.name.Split(" ")[0];
        if (++count > 4) parent.GetComponent<RectTransform>().sizeDelta += -spacing;
        PlantData pd = Resources.Load<PlantData>("PlantData/" + k.Replace(" ", "") + "Data");
        if(pd) newBtn.GetComponent<Button>().onClick.AddListener(() => Plant(pd));
        else newBtn.GetComponent<Button>().onClick.AddListener(() => Plant(k));
        List<TMP_Text> texts = newBtn.transform.GetComponentsInChildren<TMP_Text>().ToList();
        texts[0].text = newBtn.name;
        texts[1].text = v.ToString();
        //if (v == 0) AbleButton(newBtn, false);
        newBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GameManager.Inventory.TryGet(k, out int ret))
                newBtn.transform.Find("Count").gameObject.GetComponent<TMP_Text>().text = ret.ToString();
            if (ret == 0) AbleButton(newBtn, false);
        });
    }
    public void CreateInventoryIcon(string k, int v, int count, int value, Sprite image)
    {
        GameObject newIcon = Instantiate(iconPF, inventoryParent);
        newIcon.transform.localPosition = iOffset + iSpacing * count;
        newIcon.name = k;
        if (++count > 5) inventoryParent.GetComponent<RectTransform>().sizeDelta += iSpacing;
        Image newImage = newIcon.transform.Find("Image").GetComponent<Image>();
        newImage.sprite = image;
        newImage.preserveAspect = true;
        newIcon.transform.Find("Count").GetComponent<TMP_Text>().text = "x" + v.ToString();
        Transform sellBtn = newIcon.transform.Find("SellButton");
        sellBtn.GetComponentInChildren<TMP_Text>().text = "Sell: " + value;
        sellBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            string key = k[..k.LastIndexOf(" ")];
            if (GameManager.Inventory.TryGet(key, out _))
                GameManager.Inventory[key]--;
            InventorySell(newIcon);
        });
    }
    public void InventorySell(GameObject icon)
    {
        int count = 0;
        if(icon.name.Contains("Cat"))
        {
            Cat catToSell = catsParent.transform.Find(icon.name).GetComponent<Cat>();
            count = catsParent.GetComponentsInChildren<Transform>().Where(c => c.name == icon.name).Count();
            if (!GameManager.instance.IsQuickSell())
            {
                GameManager.instance.ObjectToConfirm(catToSell.gameObject);
                GameManager.instance.WaitForConfirmation("Cat.Sell");
                GameManager.instance.currentCoroutines.Add(StartCoroutine(DelayForConfirm(() =>
                    icon.transform.Find("Count").GetComponent<TMP_Text>().text = "x" + count--.ToString())));
                //GameManager.instance.currentCoroutines.Add(StartCoroutine(DelayForConfirm(() =>
                //    GameManager.instance.Inventory[icon.name.Split(" ")[0]] = count)));
                if(count == 0)
                    GameManager.instance.currentCoroutines.Add(StartCoroutine(DelayForConfirm(() =>
                        AbleButton(icon.transform.Find("SellButton").gameObject, false))));
            }
            else
            {
                GameManager.instance.Confirm();
                StartCoroutine(catToSell.Sell());
                icon.transform.Find("Count").GetComponent<TMP_Text>().text = "x" + count--.ToString();
                if(count == 0)
                    AbleButton(icon.transform.Find("SellButton").gameObject, false);
            }
        }
        else
        {
            string key = icon.name[..icon.name.LastIndexOf(" ")];
            count = GameManager.Inventory[key];
            int value = int.Parse(icon.transform.Find("SellButton").GetComponentInChildren<TMP_Text>().text[5..]);
            GameManager.instance.AddCoin(value);
            icon.transform.Find("Count").GetComponent<TMP_Text>().text = "x" + count.ToString();
            if (count == 0)
                AbleButton(icon.transform.Find("SellButton").gameObject, false);
            GameManager.Inventory[key] = count;
            UpdateButtons(key);
        }
    }
    private void AbleButton(GameObject btn, bool enable)
    {
        btn.GetComponent<Button>().enabled = enable;
        btn.GetComponent<Image>().color *= enable ? 2 : .5f;
    }
    public void PopulateButtons(InventoryStruct invo)
    {
        int seedCount = 0, csCount = 0, invoCount = 0;
        List<Transform> cats = catsParent.transform.GetComponentsInChildren<Transform>().Skip(1).ToList();
        foreach (Transform cat in cats.Distinct())
            CreateInventoryIcon(cat.name, cats.Where(ct => ct.name == cat.name).Count(),
                invoCount++, cat.GetComponent<Cat>().stats.value, cat.GetComponent<SpriteRenderer>().sprite);
        foreach (var s in invo.seeds)
        {
            CreateMenuButton(s.Key, s.Value, seedSelectParent, seedCount++);
            CreateInventoryIcon(s.Key + " Seed", s.Value, invoCount++, 5, GetIconSprite(s.Key));
        }
        foreach (var c in invo.catsules)
        {
            CreateMenuButton(c.Key, c.Value, catsuleSelectParent, csCount++);
            CreateInventoryIcon(c.Key + " Catsule", c.Value, invoCount++, 25, GetIconSprite("Catsule_Icon"));
        }
        foreach (var d in invo.decors)
            CreateInventoryIcon(d.Key, d.Value, invoCount++, 0, GetIconSprite(d.Key));
    }
    private Sprite GetIconSprite(string name)
    {
        Sprite spt = Resources.Load<PlantData>("PlantData/" + name.Replace(" ", "") + "Data")?
            .plantPrefab.GetComponent<SpriteRenderer>().sprite;
        if (spt) return spt;
        spt = Resources.Load<Sprite>("Sprites/Catsule_Icon");
        if(spt) return spt;
        spt = Resources.Load<Sprite>("Sprites/" + name);
        if (spt) return spt;
        return null;
    }
    public void UpdateButtons(string name)
    {
        Transform parent = FindParent(name, out Transform btn);
        if(parent == null) return;
        //Transform btn = parent.Find(name + " " + parent.name.Split(" ")[0]);
        if (btn && GameManager.Inventory.TryGet(name, out int iCount))
        {
            btn.Find("Count").gameObject.GetComponent<TMP_Text>().text = iCount.ToString();
            if (iCount == 0) AbleButton(btn.gameObject, false);
            else if (!btn.GetComponent<Button>().enabled) AbleButton(btn.gameObject, true);
        }
        else CreateMenuButton(name, 1, parent, parent.childCount);
    }
    private Transform FindParent(string name, out Transform child)
    {
        if (GameManager.Inventory.seeds.ContainsKey(name))
        {
            child = seedSelectParent.Find(name + " Seed");
            return seedSelectParent;
        }
        else if (GameManager.Inventory.catsules.ContainsKey(name))
        {
            child = catsuleSelectParent.Find(name + " Catsule");
            return catsuleSelectParent;
        }
        else
        {
            child = null;
            return null;
        }
    }
    public void UpdateIcons(string name, int value = 0)
    {
        Transform icon = inventoryParent.Find(name), btn;
        string key = name[..name.LastIndexOf(" ")];
        int count = name.Contains(" Cat") ?
            catsParent.GetComponentsInChildren<Transform>().Where(c => c.name == name).Count() :
            GameManager.Inventory[key];
        if (icon)
        {
            btn = icon.Find("SellButton");
            icon.Find("Count").GetComponent<TMP_Text>().text = "x" + count.ToString();
            if (count == 0) AbleButton(btn.gameObject, false);
            else if (!btn.GetComponent<Button>().enabled) AbleButton(btn.gameObject, true);
        }
        else CreateInventoryIcon(name, 1, inventoryParent.childCount, value, GetIconSprite(key));
    }
    public void OpenSeedUI()
    {
        if (plotSelected == null) return;
        if (catSelectUI.activeSelf) CloseCatUI();
        seedSelectBox.SetActive(true);
        seedSelectBox.GetComponent<Image>().color = Color.white;
        seedSelectUI.SetActive(true);
    }
    public void CloseSeedUI()
    {
        seedSelectBox.SetActive(false);
        StartCoroutine(DelayMenu(seedSelectUI));
    }
    public void OpenCatUI()
    {
        if (!plotSelected) return;
        if(seedSelectUI.activeSelf) CloseSeedUI();
        catsuleSelectBox.SetActive(true);
        catsuleSelectBox.GetComponent<Image>().color = Color.white;
        catSelectUI.SetActive(true);
    }
    public void CloseCatUI()
    {
        catsuleSelectBox.SetActive(false);
        StartCoroutine(DelayMenu(catSelectUI));
    }
    public void SelectEnter(GameObject UI)
    {
        Debug.Log("Plowmode: " + plowMode + "; Watermode: " + isWateringMode);
        UI.SetActive(true);
        if (!IsUIOpen(UI))
            UI.GetComponent<Image>().color *= .5f;
    }
    public void SelectExit(GameObject UI)
    {
        if (IsUIOpen(UI)) return;
        UI.GetComponent<Image>().color *= 2f;
        UI.SetActive(false);
    }
    public void WateringCanClick()
    {
        StartCoroutine(SlowToggle(!isWateringMode, (bool value) => isWateringMode = value));
        if (!isWateringMode) wateringCanSelectBox.GetComponent<Image>().color = Color.white;
        else wateringCanSelectBox.GetComponent<Image>().color *= .5f;
        plowMode = false;
        shovelSelectBox.GetComponent<Image>().color = Color.white;
        shovelSelectBox.SetActive(false);
    }
    public void ShovelClick()
    {
        Debug.Log("Plowmode is " + plowMode);
        StartCoroutine(SlowToggle(!plowMode, (bool value) => plowMode = value));
        if (!plowMode) shovelSelectBox.GetComponent<Image>().color = Color.white;
        else shovelSelectBox.GetComponent<Image>().color *= .5f;
        isWateringMode = false;
        wateringCanSelectBox.GetComponent<Image>().color = Color.white;
        wateringCanSelectBox.SetActive(false);
    }
    public IEnumerator SlowToggle(bool toggle, System.Action<bool> callback)//, GameObject selectBox)
    {
        Debug.Log("SlowToggle called.");
        yield return new WaitForSeconds(.25f);
        callback(toggle);
        Debug.Log("Plowmode should be " + toggle + " and is now " + plowMode);
        //if (toggle) selectBox.GetComponent<Image>().color = Color.white;
        //else selectBox.GetComponent<Image>().color *= .5f;
    }
    public void HideIcons()
    {
        seedSelectBox.transform.parent.gameObject.SetActive(false);
        catsuleSelectBox.transform.parent.gameObject.SetActive(false);
    }
    private bool IsUIOpen(GameObject UI)
    {
        return UI.name.Contains("Catsule") && catSelectUI.activeSelf ||
               UI.name.Contains("Seed") && seedSelectUI.activeSelf ||
               UI.name.Contains("Water") && isWateringMode ||
               UI.name.Contains("Shovel") && plowMode;
    }
    public bool IsAnyUIOpen()
    {
        //return seedSelectUI.activeSelf || catSelectUI.activeSelf || pauseMenu.activeSelf ||
        //       optionsMenu.activeSelf || shopMenu.activeSelf || inventoryMenu.activeSelf ||
        //       sellMenu.activeSelf || confirmMenu.activeSelf;
        return allMenus.Any(go => go.activeSelf);
    }
    public void CloseOpenUIs(GameObject exception)
    {
        allMenus.FindAll(UI => UI != exception).ForEach(go => go.SetActive(false));
    }
    public void ResetModes()
    {
        //isWateringMode = false;
        StartCoroutine(SlowToggle(false, (bool value) => isWateringMode = value));
        wateringCanSelectBox.GetComponent<Image>().color = Color.white;
        wateringCanSelectBox.SetActive(false);
        //plowMode = false;
        StartCoroutine(SlowToggle(false, (bool value) => plowMode = value));
        shovelSelectBox.GetComponent<Image>().color = Color.white;
        shovelSelectBox.SetActive(false);
    }
    public static IEnumerator DelayMenu(GameObject UI)
    {
        yield return new WaitForSeconds(.25f);
        UI.SetActive(false);
    }
    public IEnumerator DelayForConfirm(System.Action method)
    {
        yield return new WaitUntil(() => GameManager.instance.IsConfirmed());
        method();
    }
    private IEnumerator Wait(Animator animator)
    {
        yield return new WaitForSeconds(1f);
        animator.StopPlayback();
    }
}
