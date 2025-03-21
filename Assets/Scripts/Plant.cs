using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    public PlantData plantData;
    public string color;
    public int value, sOrder;

    private Plot plot;
    private SpriteRenderer currentSprite;
    private Collider2D plantCollider;
    private int currentGrowthStage = 0;
    private float growthTimer = 0f;
    private bool isHarvestable = false, isWatered = false;
    // Start is called before the first frame update
    void Start()
    {
        // currentSprite = GetComponent<SpriteRenderer>();
        //currentSprite.sprite = plantData.growthStages[currentGrowthStage];
        //if (color == "") color = gameObject.name.Split(" ")[0];
        GetComponent<SpriteRenderer>().sortingOrder = sOrder;
    }

    public void SetPlantData(PlantData data)
    {
        plantData = data;
        currentSprite = GetComponent<SpriteRenderer>();
        currentSprite.sprite = plantData.growthStages[currentGrowthStage];
        currentSprite.sortingOrder = 4;
        plantCollider = GetComponent<Collider2D>();
    }
    public void SetPlotReference(Plot assignedPlot)
    {
        plot = assignedPlot;
        plot.GetComponent<Collider2D>().enabled = false;
    }
    public Plot GetPlotReference() { return plot; }

    // Update is called once per frame
    void Update()
    {
        if (isWatered)
        {
            if (currentGrowthStage < 3)
            {
                growthTimer += Time.deltaTime;
            }

            if (growthTimer >= plantData.growthTime)
            {
                currentGrowthStage++;
                currentSprite.sprite = plantData.growthStages[currentGrowthStage];
                Debug.Log("Growth Stage: " + currentGrowthStage);
                growthTimer = 0f;
            }
            if (currentGrowthStage >= 3)
            {
                if (gameObject.name.Contains("Pepper") && color == "") RandomizeColor();
                Finished();
            }
            else
            {
                // Disable the collider if the plant is not fully grown
                if (plantCollider && plantCollider.enabled)
                {
                    plantCollider.enabled = false;  // Disable collider during growth
                }
            }
        }
    }
    public void Finished()
    {
        isHarvestable = true;
        if (plantCollider && !plantCollider.enabled)
            plantCollider.enabled = true;  // Enable collider when plant is fully grown
    }
    public void CheckHarvest()
    {
        if (isHarvestable)
        {
            Debug.Log("Plant Harvested!");
            StartCoroutine(Harvest(value));
        }
        else
        {
            Debug.Log("Plant is not fully grown yet.");
        }
    }
    IEnumerator Harvest(int coin = 10)
    {
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
        if (plot != null)
            plot.ReEnablePlot();
        GameManager.instance.AddCoin(coin);
        Debug.Log(color);
    }

    private void RandomizeColor()
    {
        if (plantData.growthStages.Length > 4)//Meaning a pepper.
        {
            int rand = Mathf.RoundToInt(Random.value * (plantData.growthStages.Length - 4));
            GrowthStage = rand + 3;
            currentSprite.sprite = plantData.growthStages[GrowthStage];
            color = currentSprite.sprite.name.Split("_")[1];
        }
    }
    public void Water()
    {
        if(!isWatered) plot.WaterPlot();
        isWatered = true;
    }
    public bool IsWatered() { return isWatered; }



    public int GrowthStage
    {
        get { return currentGrowthStage; }
        set { currentGrowthStage = value; }
    }
    public float Timer
    {
        get { return growthTimer; }
        set { growthTimer = value; }
    }
}