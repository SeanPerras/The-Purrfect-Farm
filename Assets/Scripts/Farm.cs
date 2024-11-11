using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class Farm : MonoBehaviour
{
    public GameObject plotPrefab;
    private GameObject plotSelected;
    public GameObject seedSelectUI;

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
            land.SetActive(false);
            Debug.Log("Plot placed!");
        }

        if (!(collidedGameObjects.Find(c => c.name.Contains("Plant")) || collidedGameObjects.Find(c => c.name.Contains("Catsule")))
        //if(collidedGameObjects.Count == 0
            && plot != null){
            plotSelected = plot;
            OpenUI();

        }
        collidedGameObjects.Clear();
    }

    private void OpenUI(){
        plotSelected.GetComponent<Collider2D>().enabled = false;
        seedSelectUI.SetActive(true);
    }

    public void PlantSeed(PlantData plantData){
        if(plotSelected != null){
            plotSelected.GetComponent<Plot>().Plant(plantData);
            plotSelected = null;
        }
        seedSelectUI.SetActive(false);
        //plotSelected.GetComponent<Collider2D>().enabled = true;
    }
}
