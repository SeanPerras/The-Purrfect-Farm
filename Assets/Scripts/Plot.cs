using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plot : MonoBehaviour
{
    private List<Vector2> positions;
    private GameObject[] adjPlots = new GameObject[4];
    private Collider2D plotCollider;
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
            GameObject go = Physics2D.OverlapCircleAll(p, 0).Select(c => c.gameObject).ToList().Find(x => x.CompareTag("Plot"));
            if (go)
            {
                adjPlots[index] = go;
                go.GetComponent<Plot>().AddPlot(gameObject, (index + 2) % 4);
            }
            else adjPlots[index] = null;
            index++;
        }

        plotCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlot(GameObject go, int ind)
    {
        adjPlots[ind] = go;
    }

    public void Plant(PlantData plantData){
        GameObject plant = Instantiate(plantData.plantPrefab, transform.position, Quaternion.identity);
        Plant plantScript = plant.GetComponent<Plant>();
        plantScript.SetPlantData(plantData);
        plantScript.SetPlotReference(this);
    }

    public void ReEnablePlot(){
        plotCollider.enabled = true;
    }
}
