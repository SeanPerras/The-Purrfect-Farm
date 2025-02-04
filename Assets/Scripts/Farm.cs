using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    private bool isWateringMode = false, plowMode = false;
    private Vector3 mousePos;
    private Vector2 offset = new(158, -35), spacing = new(0, -70),
            iOffset = new(125, -140), iSpacing = new(275, 0);

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(pPointer, new Vector2(0, pPointer.height*.07f), CursorMode.Auto);
        PopulateButtons(GameManager.instance.Inventory);
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
            isWateringMode = false;
            wateringCanSelectBox.GetComponent<Image>().color = Color.white * .5f;
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
        Debug.Log("Mouse Up!");
        Vector3 db = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List<GameObject> collidedGameObjects = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0)
                .Select(c => c.gameObject).ToList();
        GameObject cat = collidedGameObjects.Find(c => c.CompareTag("Cat"));
        collidedGameObjects.Remove(cat);
        GameObject land = collidedGameObjects.Find(c => c.CompareTag("Land"));
        collidedGameObjects.Remove(land);
        GameObject plot = collidedGameObjects.Find(c => c.CompareTag("Plot"));
        collidedGameObjects.Remove(plot);
        GameObject plant = collidedGameObjects.Find(c => c.CompareTag("Plant"));
        collidedGameObjects.Remove(plant);
        if (cat)
        { /*Do nothing*/ }
        else if ((plant || (plot && plot.GetComponent<Plot>().HasPlant())) && isWateringMode)
            plant.GetComponent<Plant>().Water();
        else if (plant && plant.TryGetComponent(out Plant pc))
            pc.CheckHarvest();
        else if (land && plowMode && !isWateringMode)
        {
            //shovel.SetActive(true);
            //shovel.transform.position = land.transform.position;
            //Animator anim = shovel.GetComponent<Animator>();
            //anim.Play("Dig Animation");
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
        GameManager.instance.RemoveCoin(5);
        Debug.Log("Plot placed!");
        return newPlot;
    }
    public void Plant(PlantData plantData)
    {
        if (plotSelected)
        {
            plotSelected.GetComponent<Plot>().Plant(plantData);
            plotSelected = null;
            GameManager.instance.Inventory[plantData.plantName]--;
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
            GameManager.instance.Inventory[color]--;
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
        List<TextMeshProUGUI> texts = newBtn.transform.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        texts[0].text = newBtn.name;
        texts[1].text = v.ToString();
        //if (v == 0) AbleButton(newBtn, false);
        newBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GameManager.instance.Inventory.TryGet(k, out int ret))// v = --ret;
                newBtn.transform.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = ret.ToString();
            if (ret == 0) AbleButton(newBtn, false);
        });
    }
    public void CreateInventoryIcon(string k, int v, Transform parent, int count, int value, Sprite image)
    {
        GameObject newIcon = Instantiate(iconPF, parent);
        newIcon.transform.localPosition = iOffset + iSpacing * count;
        newIcon.name = k;
        if (++count > 5) parent.GetComponent<RectTransform>().sizeDelta += iSpacing;
        Image newImage = newIcon.transform.Find("Image").GetComponent<Image>();
        newImage.sprite = image;
        newImage.preserveAspect = true;
        newIcon.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "x" + v.ToString();
        Transform sellBtn = newIcon.transform.Find("SellButton");
        sellBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Sell: " + value;
        sellBtn.GetComponent<Button>().onClick.AddListener(() => InventorySell(newIcon, --v));
    }
    public void InventorySell(GameObject icon, int count)
    {
        if(icon.name.Contains("Cat"))
        {
            Cat catToSell = catsParent.transform.Find(icon.name).GetComponent<Cat>();
            if (!GameManager.instance.IsQuickSell())
            {
                GameManager.instance.ObjectToConfirm(catToSell.gameObject);
                GameManager.instance.WaitForConfirmation("Cat.Sell");
                GameManager.instance.currentCoroutines.Add(StartCoroutine(DelayForConfirm(() =>
                    icon.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "x" + count.ToString())));
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
                icon.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "x" + count.ToString();
                if(count == 0)
                    AbleButton(icon.transform.Find("SellButton").gameObject, false);
            }
        }
        else if (icon.name.Contains("Seed"))
        {
            string item = icon.name[..icon.name.IndexOf(" Seed")];
            int value = int.Parse(icon.transform.Find("SellButton").GetComponentInChildren<TextMeshProUGUI>().text[5..]);
            GameManager.instance.AddCoin(value);
            icon.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "x" + count.ToString();
            if (count == 0)
                AbleButton(icon.transform.Find("SellButton").gameObject, false);
            GameManager.instance.Inventory[item] = count;
            UpdateButtons(seedSelectParent, item);
        }
        else if (icon.name.Contains("Catsule"))
        {
            string item = icon.name[..icon.name.IndexOf(" Catsule")];
            int value = int.Parse(icon.transform.Find("SellButton").GetComponentInChildren<TextMeshProUGUI>().text[5..]);
            GameManager.instance.AddCoin(value);
            icon.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "x" + count.ToString();
            if (count == 0)
                AbleButton(icon.transform.Find("SellButton").gameObject, false);
            GameManager.instance.Inventory[item] = count;
            UpdateButtons(catsuleSelectParent, item);
        }
        else if (icon.name.Contains("Decoration"))
        {
            string item = icon.name[..icon.name.IndexOf(" Decoration")];
            int value = int.Parse(icon.transform.Find("SellButton").GetComponentInChildren<TextMeshProUGUI>().text[5..]);
            GameManager.instance.AddCoin(value);
            icon.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "x" + count.ToString();
            if (count == 0)
                AbleButton(icon.transform.Find("SellButton").gameObject, false);
            GameManager.instance.Inventory[item] = count;
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
            CreateInventoryIcon(cat.name, cats.Where(x => x.name == cat.name).Count(),
                inventoryParent, invoCount++, cat.GetComponent<Cat>().stats.value,
                cat.GetComponent<SpriteRenderer>().sprite);
        foreach (var s in invo.seeds)
        {
            CreateMenuButton(s.Key, s.Value, seedSelectParent, seedCount++);
            PlantData pd = Resources.Load<PlantData>("PlantData/" + s.Key.Replace(" ", "") + "Data");
            CreateInventoryIcon(s.Key + " Seed", s.Value, inventoryParent,
                invoCount++, 5, pd.plantPrefab.GetComponent<SpriteRenderer>().sprite);
        }
        foreach (var c in invo.catsules)
        {
            CreateMenuButton(c.Key, c.Value, catsuleSelectParent, csCount++);
            CreateInventoryIcon(c.Key + " Catsule", c.Value, inventoryParent,
                invoCount++, 25, Resources.Load<Sprite>("Sprites/Catsule_Icon"));
        }
        foreach (var d in invo.decors)
            CreateInventoryIcon(d.Key, d.Value, inventoryParent,
                invoCount++, 0, Resources.Load<Sprite>("Sprites/" + d.Key));
    }
    public void UpdateButtons(Transform parent, string name)
    {
        Transform btn = parent.Find(name + " " + parent.name.Split(" ")[0]);
        if (btn && GameManager.instance.Inventory.TryGet(name, out int iCount))
        {
            btn.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = iCount.ToString();
            if (iCount == 0) AbleButton(btn.gameObject, false);
            else if (!btn.GetComponent<Button>().enabled) AbleButton(btn.gameObject, true);
        }
        else CreateMenuButton(name, 1, parent, parent.childCount);
    }
    public void UpdateIcons(string name)
    {
        GameObject icon = inventoryParent.Find(name).gameObject;
        Transform btn = icon.transform.Find("SellButton");
        int count = name.Contains("Cat") ?
            catsParent.GetComponentsInChildren<Transform>().Skip(1).Where(c => c.name == name).Count() :
            GameManager.instance.Inventory[name.Split(" ")[0]];
        if (icon)
        {
            icon.transform.Find("Count").GetComponent<TextMeshProUGUI>().text = "x" + count.ToString();
            if (count == 0) AbleButton(btn.gameObject, false);
            else if (!btn.GetComponent<Button>().enabled) AbleButton(btn.gameObject, true);
        }
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
        isWateringMode = !isWateringMode;
        plowMode = isWateringMode;
        if (isWateringMode) wateringCanSelectBox.GetComponent<Image>().color = Color.white;
        else wateringCanSelectBox.GetComponent<Image>().color *= .5f;
    }
    public void ShovelClick()
    {
        StartCoroutine(SlowToggle(!plowMode, (bool value) => plowMode = value));
    }
    public IEnumerator SlowToggle(bool toggle, System.Action<bool> callback)
    {
        yield return new WaitForSeconds(.25f);
        callback(toggle);
        if (plowMode) shovelSelectBox.GetComponent<Image>().color = Color.white;
        else shovelSelectBox.GetComponent<Image>().color *= .5f;
    }
    public void HideIcons()
    {
        seedSelectBox.transform.parent.gameObject.SetActive(false);
        catsuleSelectBox.transform.parent.gameObject.SetActive(false);
    }
    private bool IsUIOpen(GameObject UI)
    {
        return UI.name.Contains("Catsule") && catSelectUI.activeSelf ||
               UI.name.Contains("Plant") && seedSelectUI.activeSelf ||
               UI.name.Contains("Water") && isWateringMode ||
               UI.name.Contains("Shovel") && plowMode;
    }
    public bool IsAnyUIOpen()
    {
        return seedSelectUI.activeSelf || catSelectUI.activeSelf || pauseMenu.activeSelf ||
               optionsMenu.activeSelf || shopMenu.activeSelf || inventoryMenu.activeSelf ||
               sellMenu.activeSelf || confirmMenu.activeSelf;
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
