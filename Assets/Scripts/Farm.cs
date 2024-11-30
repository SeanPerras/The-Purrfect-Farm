using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.XR;

public class Farm : MonoBehaviour
{
    public readonly Dictionary<string, List<string>> Colors = new()
    {
        {"Black",  new() {"Black", "Black"} },
        {"Blue", new () {"Blue", "Blue"} },
        {"Red", new() { "Red", "Red" } },
        {"Yellow", new() { "Yellow", "Yellow" } }
    };
    public GameObject
        plotPrefab, plotsParent, shovel,
        seedSelectUI, catSelectUI, catsuleselected, seedselected, wateringCanSelected,
        pauseMenu, optionsMenu, shopMenu, inventoryMenu;
    public Texture2D pPointer, pDrag;

    private GameObject plotSelected;
    private bool isWateringMode = false;
    private Vector3 mousePos;
    private bool plotMode = true; //This is just until we implement a proper "I want to plow." mechanic.
    //private int totalWateredPlants = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(pPointer, new Vector2(0, pPointer.Size().y*.07f), CursorMode.Auto);
    }
    // Update is called once per frame
    void Update()
    {
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
                Cursor.SetCursor(pDrag, new Vector2(pDrag.Size().x * .5f, pDrag.Size().y * .5f), CursorMode.Auto);
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
        if (isWateringMode && plotsParent.transform.GetComponentsInChildren<Plot>().All(x => !x.IsWaterable()))
        {
            isWateringMode = false;
            plotMode = true;
            wateringCanSelected.GetComponent<Image>().color = Color.white * .5f;
            wateringCanSelected.SetActive(false);
        }
    }

    private void HandleClick()
    {
        Debug.Log("Mouse Up!");
        if (!IsAnyUIOpen())
        {
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
            { }
            else if ((plant || (plot && plot.GetComponent<Plot>().HasPlant())) && isWateringMode)
                plant.GetComponent<Plant>().Water();
            else if (land && plotMode && !isWateringMode)
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
                seedselected.transform.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(true);
                catsuleselected.transform.GetComponentInParent<Transform>().GetChild(1).gameObject.SetActive(true);
                OpenSeedUI();
            }
            collidedGameObjects.Clear();
        }
    }
    public GameObject Plow(GameObject landRef)
    {
        Vector3 pos = landRef.transform.position;
        GameObject newPlot = Instantiate(plotPrefab, pos, plotPrefab.transform.rotation, plotsParent.transform);
        newPlot.GetComponent<SpriteRenderer>().sortingOrder = landRef.GetComponent<SpriteRenderer>().sortingOrder;
        newPlot.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = landRef.GetComponent<SpriteRenderer>().sortingOrder;
        newPlot.GetComponent<Plot>().SetLandRef(landRef);
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
        seedselected.transform.parent.GetChild(1).gameObject.SetActive(false);
        catsuleselected.transform.parent.GetChild(1).gameObject.SetActive(false);
    }
    private bool IsUIOpen(GameObject UI)
    {
        return UI.transform.parent.transform.parent.gameObject.name.Contains("atsule") && catSelectUI.activeSelf ||
            UI.transform.parent.transform.parent.gameObject.name.Contains("lant") && seedSelectUI.activeSelf ||
            UI.transform.parent.transform.parent.gameObject.name.Contains("ater") && isWateringMode;
    }
    public bool IsAnyUIOpen()
    {
        return seedSelectUI.activeSelf || catSelectUI.activeSelf || pauseMenu.activeSelf ||
               optionsMenu.activeSelf || shopMenu.activeSelf || inventoryMenu.activeSelf;
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
