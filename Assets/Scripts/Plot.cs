using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static SaveData;

public class Plot : MonoBehaviour//, ISaveable
{
    private Plant plant;
    private Catsule catsule;
    private List<Vector2> positions;
    private GameObject[] adjPlots = new GameObject[4];
    private Collider2D plotCollider;
    private GameObject oldLand;
    //private int totalPlants = 0;
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
                if(adjPlots[index]) adjPlots[index].GetComponent<Plot>().AddPlotRef(gameObject, (index + 2) % 4);
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

    public void AddPlotRef(GameObject go, int ind) { adjPlots[ind] = go; }
    public Plant Plant(PlantData plantData)
    {
        plant = Instantiate(plantData.plantPrefab, transform.position, Quaternion.identity).GetComponent<Plant>();
        plant.SetPlantData(plantData);
        plant.SetPlotReference(this);
        plant.gameObject.name = plantData.plantPrefab.name;
        //plant.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        plant.GetComponent<SpriteRenderer>().sortingOrder = 20;//temp fix
        return plant;
    }
    public Catsule Plant(string color = "White")
    {
        GameObject prefab = GameManager.instance.catsulePrefabs.Find(c => c.GetComponent<Catsule>().color == color);
        Catsule ct = Instantiate(prefab, transform.position, transform.rotation).GetComponent<Catsule>();
        catsule = ct;
        catsule.SetPlotReference(this);
        //catsule.gameObject.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        //catsule.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        catsule.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 20;
        catsule.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 20;//temp fix
        return catsule;
    }
    public void ReEnablePlot(){
        //GetComponent<SpriteRenderer>().color /= .9f;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        plotCollider.enabled = true;
        plant = null;
        catsule = null;
    }
    public GameObject[] GetAdjPlots() { return adjPlots; }    //{ TopRight, TopLeft, BottomLeft, BottomRight }
    public string GetColor()
    {
        if (plant) return plant.color;
        else if (catsule) return catsule.color;
        else return "White";
    }
    public bool HasPlant() { return  plant != null; }
    public bool HasCatsule() { return catsule != null; }
    public void WaterPlot()
    {
        //GetComponent<SpriteRenderer>().color *= .9f;
        Color parCol = GetComponent<SpriteRenderer>().color;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = parCol;
    }
    public bool IsPlantWatered() { return HasPlant() && plant.IsWatered(); }
    public bool IsWaterable() { return HasPlant() && !plant.IsWatered(); }
    public void SetLandRef(GameObject land)
    {
        oldLand = land;
        land.SetActive(false);
    }
    public void DeletePlot()
    {
        if (plant == null)
        {
            oldLand.SetActive(true);
            Destroy(gameObject);
        }
        else Debug.Log("Harvest or delete the plant before deleting the plot!");
    }



    public Plant GetPlant() { return plant; }
    public Catsule GetCatsule() { return catsule; }
}
