using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plot : MonoBehaviour
{
    private Plant plant;
    private Catsule catsule;
    private List<Vector2> positions;
    private GameObject[] adjPlots = new GameObject[4];
    private Collider2D plotCollider;
    //privtate int totalPlants = 0;
    //public GameObject plantPrefab;
    // Start is called before the first frame update
    void Start()
    {
        positions = new() {
            transform.position + new Vector3(5f, 2.5f),//topright
            transform.position + new Vector3(-5f, 2.5f),//topleft
            transform.position + new Vector3(-5f, -2.5f),//botleft
            transform.position + new Vector3(5f, -2.5f) };//botright
        int index = 0;
        foreach (Vector2 p in positions)
        {
            GameObject go;
            try { go = Physics2D.OverlapCircleAll(p, 0).Select(c => c.gameObject).ToList()[0]; }
            catch { index++; continue; }
            if (go.CompareTag("Plot") || go.name.Contains("Plant") || go.name.Contains("Catsule"))
            {
                adjPlots[index] = go.CompareTag("Plot") ? go :
                    go.name.Contains("Plant") ? adjPlots[index] = go.GetComponent<Plant>().GetPlotReference().gameObject :
                    go.name.Contains("Catsule") ? adjPlots[index] = go.GetComponent<Catsule>().GetPlotReference().gameObject :
                    null;
                if(adjPlots[index]) adjPlots[index].GetComponent<Plot>().AddPlot(gameObject, (index + 2) % 4);
            }
            index++;
        }

        plotCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseUp()
    {
        Debug.Log("Plot clicked.");
    }

    public void AddPlot(GameObject go, int ind)
    {
        adjPlots[ind] = go;
    }

    public void Plant(PlantData plantData){
        plant = Instantiate(plantData.plantPrefab, transform.position, Quaternion.identity).GetComponent<Plant>();
        //Plant plantScript = plant.GetComponent<Plant>();
        //plantScript.SetPlantData(plantData);
        //plantScript.SetPlotReference(this);
        plant.SetPlantData(plantData);
        plant.SetPlotReference(this);
    }
    public void Plant(Catsule ct)
    {
        catsule = ct;
        catsule.SetPlotReference(this);
    }

    public void ReEnablePlot(){
        //GetComponent<SpriteRenderer>().color /= .9f;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        plotCollider.enabled = true;
        plant = null;
        catsule = null;
    }
    public GameObject[] GetAdjPlots()
    {
        return adjPlots;    //{ TopRight, TopLeft, BottomLeft, BottomRight }
    }
    public string GetColor()
    {
        if (plant) return plant.color;
        else if (catsule) return catsule.color;
        else return "White";
    }
    public bool HasPlant() { return  plant != null; }
    public void WaterPlot()
    {
        //GetComponent<SpriteRenderer>().color *= .9f;
        Color parCol = GetComponent<SpriteRenderer>().color;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = parCol;
    }
}
