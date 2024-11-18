using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Farm : MonoBehaviour
{
    public readonly Dictionary<string, List<string>> Colors = new()
    {
        {"Black",  new() {"Black", "Black"} },
        {"Blue", new () {"Blue", "Blue"} },
        {"Red", new() { "Red", "Red" } },
        {"Yellow", new() { "Yellow", "Yellow" } }
    };
    public GameObject plotPrefab;
    private GameObject plotSelected;
    public GameObject seedSelectUI;
    public GameObject catsulePrefab;

    
    public GameObject catSelectUI;
    public GameObject plotsParent;
    private bool isSeedSelectUI = false;
    private bool isCatSelectUI = false;
    private bool isWateringMode = false;
    public GameObject catsuleselected;
    public GameObject seedselected;
    public GameObject wateringCanSelected;
    public GameObject pauseMenu;
    public GameObject optionsMenu;


    private Vector3 mousePos;
    private bool plotMode = true; //This is just until we implement a proper "I want to plow." mechanic.
    //private int totalWateredPlants = 0;
    public GameObject shovel;
    public Texture2D pPointer, pDrag;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(pPointer, new Vector2(0, pPointer.Size().y*.07f), CursorMode.Auto);
    }
    // Update is called once per frame
    void Update()
    {
        if(pauseMenu.activeSelf){
            return;
        }
        if(optionsMenu.activeSelf){
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
                HandleClick();
        }
        else if (Input.GetMouseButtonDown(1))
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else if (Input.GetMouseButton(1))
        {
            if (mousePos != Vector3.zero && Camera.main.ScreenToWorldPoint(Input.mousePosition) != mousePos)
            {
                Cursor.SetCursor(pDrag, Vector2.zero, CursorMode.Auto);
                Camera.main.transform.position -= Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;
            }
        }
        else if (Input.GetMouseButtonUp(1))
            Cursor.SetCursor(pPointer, new Vector2(0, pPointer.Size().y * .07f), CursorMode.Auto);
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * 5, 10, 25);
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 5, 10, 25);
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 5, 10, 25);
        }
        if (isWateringMode && GameObject.Find("Plots").transform.GetComponentsInChildren<Plot>().All(x => x.IsPlantWatered()))
        {
            isWateringMode = false;
            plotMode = true;
            wateringCanSelected.SetActive(false);
        }
    }
    private void HandleClick()
    {
        Debug.Log("Mouse Up!");
        if (!seedSelectUI.activeSelf && !catSelectUI.activeSelf)
        {
            Vector3 db = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            List<GameObject> collidedGameObjects = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0)
                    .Select(c => c.gameObject).ToList();
            GameObject cat = collidedGameObjects.Find(c => c.name.Contains(" Cat"));
            collidedGameObjects.Remove(cat);
            GameObject land = collidedGameObjects.Find(c => c.name == "Land");
            collidedGameObjects.Remove(land);
            GameObject plot = collidedGameObjects.Find(c => c.CompareTag("Plot"));
            collidedGameObjects.Remove(plot);
            GameObject plant = collidedGameObjects.Find(c => c.name.Contains("Plant"));
            collidedGameObjects.Remove(plant);
            if (cat) { }
            else if ((plant || (plot && plot.GetComponent<Plot>().HasPlant())) && isWateringMode)
                plant.GetComponent<Plant>().Water();
            else if (land && plotMode)
            {
                Vector3 pos = land.transform.position;
                //shovel.SetActive(true);
                //shovel.transform.position = pos;
                //shovel.GetComponent<Animator>().Play("Dig Animation");
                GameObject newPlot = Instantiate(plotPrefab, pos, plotPrefab.transform.rotation);
                newPlot.transform.SetParent(GameObject.Find("Plots").transform);
                newPlot.GetComponent<Plot>().SetLandRef(land);
                Debug.Log("Plot placed!");
            }
            else if (collidedGameObjects.Count == 0 && plot && !isWateringMode)
            {
                plotSelected = plot;
                seedselected.transform.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(true);
                catsuleselected.transform.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(true);
                OpenSeedUI();
            }
            collidedGameObjects.Clear();
        }
    }
    public void PlantSeed(PlantData plantData)
    {
        if (plotSelected != null)
        {
            plotSelected.GetComponent<Plot>().Plant(plantData);
            plotSelected = null;
        }
        CloseSeedUI();
        if (!catSelectUI.activeSelf) HideIcons();
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
    public void PlantCatsule()
    {
        if (plotSelected != null)
        {
            GameObject catsule = Instantiate(catsulePrefab, plotSelected.transform.position, plotSelected.transform.rotation);
            plotSelected.GetComponent<Plot>().Plant(catsule.GetComponent<Catsule>());
            //plotSelected.GetComponent<Collider2D>().enabled = true;
            plotSelected = null;
        }
        CloseCatUI();
        if (!seedSelectUI.activeSelf) HideIcons();
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
    public void AddCoin(int coin)
    {
        //Add coin amount to currency;
    }

    public void OpenSeedUI()
    {
        if (plotSelected == null) return;
        if (isCatSelectUI) CloseCatUI();
        seedselected.SetActive(true);
        seedselected.GetComponent<Image>().color = Color.white;

        seedSelectUI.SetActive(true);  
        isSeedSelectUI = true;

        Debug.Log("seedselect open");
    }
    public void CloseSeedUI()
    {
        seedselected.SetActive(false);
        isSeedSelectUI = false;
        StartCoroutine(DelayMenu(seedSelectUI));
    }
    public void OpenCatUI()
    {
        if (!plotSelected) return;
        if(isSeedSelectUI) CloseSeedUI();
        catsuleselected.SetActive(true);
        catsuleselected.GetComponent<Image>().color = Color.white;

        catSelectUI.SetActive(true);
        isCatSelectUI = true;

        Debug.Log("seedselect open");
    }
    public void CloseCatUI()
    {
        catsuleselected.SetActive(false);
        isCatSelectUI = false;
        StartCoroutine(DelayMenu(catSelectUI));
    }
    private bool IsClickOverUI(GameObject uiElement)
    {

        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            Input.mousePosition,
            Camera.main);

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
        plotMode = isWateringMode;
        if (isWateringMode) wateringCanSelected.GetComponent<Image>().color = Color.white;
        else wateringCanSelected.GetComponent<Image>().color *= .5f;
    }
    public void HideIcons()
    {
        seedselected.transform.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(false);
        catsuleselected.transform.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(false);
    }
    private bool IsUIOpen(GameObject UI)
    {
        return UI.transform.parent.transform.parent.gameObject.name.Contains("atsule") && isCatSelectUI ||
            UI.transform.parent.transform.parent.gameObject.name.Contains("lant") && isSeedSelectUI ||
            UI.transform.parent.transform.parent.gameObject.name.Contains("ater") && isWateringMode;
    }
    IEnumerator DelayMenu(GameObject UI)
    {
        yield return new WaitForSeconds(.25f);
        UI.SetActive(false);
    }
}
