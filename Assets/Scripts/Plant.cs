using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    public PlantData plantData;
    private SpriteRenderer currentSprite;
    private int currentGrowthStage = 0;
    private float growthTimer = 0f;
    private Plot plot;
    private bool hasBeenHarvested = false;
    private bool isHarvestable = false;
    private Collider2D plantCollider;
    // Start is called before the first frame update
    void Start()
    {
       // currentSprite = GetComponent<SpriteRenderer>();
        //currentSprite.sprite = plantData.growthStages[currentGrowthStage];
        
    }

    public void SetPlantData(PlantData data)
    {
        plantData = data;
        currentSprite = GetComponent<SpriteRenderer>();
        currentSprite.sprite = plantData.growthStages[currentGrowthStage];
         plantCollider = GetComponent<Collider2D>();
    }

    public void SetPlotReference(Plot assignedPlot)
    {
        plot = assignedPlot;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentGrowthStage < plantData.growthStages.Length - 1){
            growthTimer += Time.deltaTime;
        }

        if(growthTimer >= plantData.growthTime[currentGrowthStage]){
            currentGrowthStage++;
            currentSprite.sprite = plantData.growthStages[currentGrowthStage];
            Debug.Log("Growth Stage: " + currentGrowthStage); 
            growthTimer = 0f;
        }

        if (currentGrowthStage == plantData.growthStages.Length - 1)
    {
        isHarvestable = true;
        if (plantCollider != null && !plantCollider.enabled)
        {
            plantCollider.enabled = true;  // Enable collider when plant is fully grown
        }
    }
    else
    {
        // Disable the collider if the plant is not fully grown
        if (plantCollider != null && plantCollider.enabled)
        {
            plantCollider.enabled = false;  // Disable collider during growth
        }
    }
        
    }



void OnMouseDown(){
    if (isHarvestable && !hasBeenHarvested)
        {
            Debug.Log("Plant Harvested!");
            Harvest();
        }
        else if (!isHarvestable)
        {
            Debug.Log("Plant is not fully grown yet.");
        }
}


void Harvest(){
    Destroy(gameObject);
    if(plot != null){
    plot.ReEnablePlot();
    }
    //hasBeenHarvested = true;
}

}