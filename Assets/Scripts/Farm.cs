using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Vector3 mousePos;
    private bool plotMode = true;

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
            Debug.DrawLine(db - new Vector3(.1f, 0, 0), db + new Vector3(.1f, 0, 0), Color.red, 10f, false);
            List<GameObject> collidedGameObjects = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), .1f)
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
                land.SetActive(false);
                Debug.Log("Plot placed!");
            }
            //if (!(collidedGameObjects.Find(c => c.name.Contains("Plant")) || collidedGameObjects.Find(c => c.name.Contains("Catsule")))
            if (collidedGameObjects.Count == 0
                && plot != null)
            {
                plotSelected = plot;
                OpenUI();

            }
            collidedGameObjects.Clear();
        }
    }

    private void OpenUI(){
        plotSelected.GetComponent<Collider2D>().enabled = false;
        seedSelectUI.SetActive(true);
    }

    private void PlantSeed(PlantData plantData){
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
            plotSelected.GetComponent<Collider2D>().enabled = true;
            plotSelected = null;
        }
        StartCoroutine(DelayMenu());
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
}
