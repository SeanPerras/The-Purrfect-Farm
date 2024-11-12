using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

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
    public GameObject catsuleselected;
    public GameObject seedselected;


    private Vector3 mousePos;
    private bool plotMode = true; //This is just until we implement a proper "I want to plow." mechanic.

    // Start is called before the first frame update
    void Start()
    {

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
                Camera.main.transform.position -= Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;
        }
        else if (Input.GetMouseButtonUp(1))
            mousePos = Vector3.zero;

    }
    private void HandleClick()
    {
        Debug.Log("Mouse Up!");
        if (seedSelectUI.activeSelf == false)
        {
            Vector3 db = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            List<GameObject> collidedGameObjects = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0)
                    .Select(c => c.gameObject).Where(x => !x.name.Contains("(9x9)")).ToList();
            GameObject land = collidedGameObjects.Find(c => c.name == "Land");
            collidedGameObjects.Remove(land);
            GameObject plot = collidedGameObjects.Find(c => c.CompareTag("Plot")); //Im thinking do the planting of seeds the same way as this, where I look for the Plot name and then it pops up the UI element to plant a seed.
            collidedGameObjects.Remove(plot);
            if (land != null && plotMode)
            {
                Vector3 pos = land.transform.position;
                GameObject newPlot = Instantiate(plotPrefab, pos, plotPrefab.transform.rotation);
                newPlot.transform.SetParent(GameObject.Find("Plots").transform);
                plotSelected = newPlot;
                OpenUI();
                land.SetActive(false);
                Debug.Log("Plot placed!");
            }
            else if (collidedGameObjects.Count == 0 && plot != null)
            {
                plotSelected = plot;
                OpenUI();
            }
            collidedGameObjects.Clear();
        }
    }

    public void OpenUI(){
        seedselectenter();
        CloseUIcat();

        seedSelectUI.SetActive(true);  
        isSeedSelectUI = true;

        Collider2D[] plotColliders = plotsParent.GetComponentsInChildren<Collider2D>();

        if (plotColliders.Length == 0)
        {
            Debug.LogWarning("No Collider2D components found in child objects of plotsParent.");
        }

       
        foreach (Collider2D collider in plotColliders)
        {
            if (collider.gameObject.CompareTag("Plot")) 
            { 
                collider.enabled = false; 

            }
        }

        Debug.Log("seedselect open");


    }
    public void CloseUI()
    {
        seedselected.SetActive(false);
        seedselectexit();
        seedSelectUI.SetActive(false);
        isSeedSelectUI = false;
        Debug.Log("zsxdfcgvbhjnkml,;.'xetcfyvgubhnjmk,l");

        Collider2D[] plotColliders = plotsParent.GetComponentsInChildren<Collider2D>();

        if (plotColliders.Length == 0)
        {
            Debug.LogWarning("No Collider2D components found in child objects of plotsParent.");
        }


        foreach (Collider2D collider in plotColliders)
        {
            if (collider.gameObject.CompareTag("Plot"))
            {
                collider.enabled = true;

            }
        }




    }
    private bool IsClickOverUI(GameObject uiElement)
    {

        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            Input.mousePosition,
            Camera.main);

    }

    public void PlantSeed(PlantData plantData){
        if(plotSelected != null){
            plotSelected.GetComponent<Plot>().Plant(plantData);
            plotSelected = null;
        }
        StartCoroutine(DelayMenu());
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
    IEnumerator DelayMenu()
    {
        yield return new WaitForSeconds(.25f);
        seedSelectUI.SetActive(false);
    }

    public void PlantCatsule(){
        if(plotSelected != null){
            GameObject catsule = Instantiate(catsulePrefab, plotSelected.transform.position, plotSelected.transform.rotation);
            plotSelected.GetComponent<Plot>().Plant(catsule.GetComponent<Catsule>());
            //plotSelected.GetComponent<Collider2D>().enabled = true;
            plotSelected = null;
        }
        StartCoroutine(DelayMenu());
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }

    public void OpenUIcat()
    {

        CloseUI();


        catSelectUI.SetActive(true);
        isCatSelectUI = true;


        Collider2D[] plotColliders = plotsParent.GetComponentsInChildren<Collider2D>();

        if (plotColliders.Length == 0)
        {
            Debug.LogWarning("No Collider2D components found in child objects of plotsParent.");
        }


        foreach (Collider2D collider in plotColliders)
        {
            if (collider.gameObject.CompareTag("Plot"))
            {
                collider.enabled = false;

            }
        }

        Debug.Log("seedselect open");





    }
    public void CloseUIcat()
    {
        catsuleselected.SetActive(false);
        catSelectUI.SetActive(false);
        isCatSelectUI = false;

        Debug.Log("zsxdfcgvbhjnkml,;.'xetcfyvgubhnjmk,l");
        Collider2D[] plotColliders = plotsParent.GetComponentsInChildren<Collider2D>();

        if (plotColliders.Length == 0)
        {
            Debug.LogWarning("No Collider2D components found in child objects of plotsParent.");
        }

        foreach (Collider2D collider in plotColliders)
        {
            if (collider.gameObject.CompareTag("Plot"))
            {
                collider.enabled = true;

            }
        }


    }

    public void catsuleselectenter()
    {

        catsuleselected.SetActive(true);
    }
    public void seedselectenter()
    {

        seedselected.SetActive(true);
    }
    public void catsuleselectexit()
    {
        if (isCatSelectUI) { return; }
        catsuleselected.SetActive(false);
    }
    public void seedselectexit()
    {
        yield return new WaitForSeconds(.25f);
        seedSelectUI.SetActive(false);
    }

}
