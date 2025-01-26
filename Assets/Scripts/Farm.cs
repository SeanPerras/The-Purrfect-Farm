using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Farm : MonoBehaviour
{
    public static readonly Dictionary<string, List<string>> Colors = new()
    {
        {"Black",  new() {"Black", "Black"} },
        {"Blue", new () {"Blue", "Blue"} },
        {"Sky Blue", new () {"Blue"} },
        {"Red", new() { "Red", "Red" } },
        {"Purple", new() { "Red", "Blue" } },
        {"Pink", new() { "Red" } },
        {"Yellow", new() { "Yellow", "Yellow" } },
        {"Orange", new() { "Red", "Yellow" } }
    };
    public GameObject
        plotPrefab, buttonPF, plotsParent, seedSelectParent, catsuleSelectParent, shovel,
        seedSelectUI, catSelectUI, catsuleselected, seedselected, wateringCanSelected, shovelSelected,
        pauseMenu, optionsMenu, shopMenu, inventoryMenu, sellMenu;//, allMenus;
    public Texture2D pPointer, pDrag;

    private GameObject plotSelected;
    private bool isWateringMode = false, plowMode = false;
    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(pPointer, new Vector2(0, pPointer.height*.07f), CursorMode.Auto);
        PopulateButtons(GameManager.instance.Inventory.seeds, GameManager.instance.Inventory.catsules);
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
            wateringCanSelected.GetComponent<Image>().color = Color.white * .5f;
            wateringCanSelected.SetActive(false);
        }
        else if(plowMode && transform.GetComponentsInChildren<Transform>().All(go => !go.gameObject.activeSelf))
        {
            plowMode = false;
            shovelSelected.GetComponent<Image>().color = Color.white * .5f;
            shovelSelected.SetActive(false);
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
        else if (collidedGameObjects.Count == 0 && plot && !isWateringMode)
        {
            plotSelected = plot;
            seedselected.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(true);
            catsuleselected.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(true);
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
    public void PlantSeed(PlantData plantData)
    {
        if (plotSelected)
        {
            plotSelected.GetComponent<Plot>().Plant(plantData);
            plotSelected = null;
        }
        CloseSeedUI();
        if (!catSelectUI.activeSelf) HideIcons();
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
    public void PlantCatsule(string color = "White")
    {
        if (plotSelected)
        {
            //GameObject prefab = GameManager.instance.catsulePrefabs.Find(c => c.GetComponent<Catsule>().color == color);
            //GameObject catsule = Instantiate(prefab, plotSelected.transform.position, plotSelected.transform.rotation);
            //plotSelected.GetComponent<Plot>().Plant(catsule.GetComponent<Catsule>());
            plotSelected.GetComponent<Plot>().Plant(color);
            //plotSelected.GetComponent<Collider2D>().enabled = true;
            plotSelected = null;
        }
        CloseCatUI();
        if (!seedSelectUI.activeSelf) HideIcons();
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
    public void SetSelectedPlot(GameObject plot) { plotSelected = plot; }
    public void CreateMenuButton(string k, int v, Transform parent, Vector2 offset, int count, string category)
    {
        GameObject newBtn = Instantiate(buttonPF, parent);
        newBtn.transform.localPosition = offset + new Vector2(0, -70 * count);
        newBtn.name = k + " " + category;
        if (category == "Seed")
        {
            if (++count > 4) seedSelectParent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 70);
            PlantData pd = Resources.Load<PlantData>("PlantData/" + k.Replace(" ", "") + "Data");
            newBtn.GetComponent<Button>().onClick.AddListener(() => PlantSeed(pd));
        }
        else if (category == "Catsule")
        {
            if (++count > 4) catsuleSelectParent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 70);
            newBtn.GetComponent<Button>().onClick.AddListener(() => PlantCatsule(k));
        }
        List<TextMeshProUGUI> texts = newBtn.transform.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        texts[0].text = newBtn.name;
        texts[1].text = v.ToString();
        if (v == 0) AbleButton(newBtn, false);
        newBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            if(GameManager.instance.Inventory.seeds.ContainsKey(k))
                v = --GameManager.instance.Inventory.seeds[k];
            else if (GameManager.instance.Inventory.catsules.ContainsKey(k))
                v = --GameManager.instance.Inventory.catsules[k];
            else if (GameManager.instance.Inventory.decors.ContainsKey(k))
                v = --GameManager.instance.Inventory.decors[k];
            newBtn.transform.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = v.ToString();
            if (v == 0) AbleButton(newBtn, false);
        });
    }
    private void AbleButton(GameObject btn, bool enable)
    {
        btn.GetComponent<Button>().enabled = enable;
        btn.GetComponent<Image>().color *= enable ? 2 : .5f;
    }
    public void PopulateButtons(Dictionary<string, int> seeds, Dictionary<string, int> catsules)
    {
        int seedCount = seedSelectParent.transform.childCount, csCount = catsuleSelectParent.transform.childCount;
        Vector2 offset = new(158, -35);
        foreach (var s in seeds)
            CreateMenuButton(s.Key, s.Value, seedSelectParent.transform, offset, seedCount++, "Seed");
        foreach(var c in catsules)
            CreateMenuButton(c.Key, c.Value, catsuleSelectParent.transform, offset, csCount++, "Catsule");
    }
    public void UpdateButtons(bool newBtn, Transform parent)
    {
        if (parent.parent.parent.parent.parent.name.Contains("Seed"))
        {
            if (!newBtn)
                foreach (Transform button in seedSelectParent.transform)
                {
                    int inventoryCount = GameManager.instance.Inventory.seeds[button.name[..button.name.IndexOf(" Seed")]];
                    string btnCount = button.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text;
                    if (int.Parse(btnCount) != inventoryCount)
                    {
                        button.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = inventoryCount.ToString();
                        if (!button.GetComponent<Button>().enabled) AbleButton(button.gameObject, true);
                    }
                }
            else CreateMenuButton(GameManager.instance.Inventory.seeds.Last().Key, GameManager.instance.Inventory.seeds.Last().Value,
                                  parent, new(158, -35), parent.childCount, "Seed");
        }
        else if (parent.parent.parent.parent.parent.name.Contains("Catsule"))
        {
            if (!newBtn)
                foreach (Transform button in catsuleSelectParent.transform)
                {
                    int inventoryCount = GameManager.instance.Inventory.catsules[button.name[..button.name.IndexOf(" Catsule")]];
                    string btnCount = button.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text;
                    if (int.Parse(btnCount) != inventoryCount)
                    {
                        button.Find("Count").gameObject.GetComponent<TextMeshProUGUI>().text = inventoryCount.ToString();
                        if (!button.GetComponent<Button>().enabled) AbleButton(button.gameObject, true);
                    }
                }
            else CreateMenuButton(GameManager.instance.Inventory.catsules.Last().Key, GameManager.instance.Inventory.catsules.Last().Value,
                                  parent, new(158, -35), parent.childCount, "Catsule");
        }
    }
    public void OpenSeedUI()
    {
        if (plotSelected == null) return;
        if (catSelectUI.activeSelf) CloseCatUI();
        seedselected.SetActive(true);
        seedselected.GetComponent<Image>().color = Color.white;

        seedSelectUI.SetActive(true);

        Debug.Log("seedselect open");
    }
    public void CloseSeedUI()
    {
        seedselected.SetActive(false);
        StartCoroutine(DelayMenu(seedSelectUI));
    }
    public void OpenCatUI()
    {
        if (!plotSelected) return;
        if(seedSelectUI.activeSelf) CloseSeedUI();
        catsuleselected.SetActive(true);
        catsuleselected.GetComponent<Image>().color = Color.white;

        catSelectUI.SetActive(true);

        Debug.Log("seedselect open");
    }
    public void CloseCatUI()
    {
        catsuleselected.SetActive(false);
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
        if (isWateringMode) wateringCanSelected.GetComponent<Image>().color = Color.white;
        else wateringCanSelected.GetComponent<Image>().color *= .5f;
    }
    public void ShovelClick()
    {
        plowMode = !plowMode;
        if (plowMode) shovelSelected.GetComponent<Image>().color = Color.white;
        else shovelSelected.GetComponent<Image>().color *= .5f;
    }
    public void HideIcons()
    {
        seedselected.transform.parent.GetChild(1).gameObject.SetActive(false);
        catsuleselected.transform.parent.GetChild(1).gameObject.SetActive(false);
    }
    private bool IsUIOpen(GameObject UI)
    {
        return UI.transform.parent.gameObject.name.Contains("Catsule") && catSelectUI.activeSelf ||
            UI.transform.parent.gameObject.name.Contains("Plant") && seedSelectUI.activeSelf ||
            UI.transform.parent.gameObject.name.Contains("Water") && isWateringMode ||
            UI.transform.parent.gameObject.name.Contains("Shovel") && plowMode;
    }
    public bool IsAnyUIOpen()
    {
        return seedSelectUI.activeSelf || catSelectUI.activeSelf || pauseMenu.activeSelf ||
               optionsMenu.activeSelf || shopMenu.activeSelf || inventoryMenu.activeSelf ||
               sellMenu.activeSelf;
    }
    public static IEnumerator DelayMenu(GameObject UI)
    {
        yield return new WaitForSeconds(.25f);
        UI.SetActive(false);
    }
    private IEnumerator Wait(Animator animator)
    {
        yield return new WaitForSeconds(1f);
        animator.StopPlayback();
    }
}
