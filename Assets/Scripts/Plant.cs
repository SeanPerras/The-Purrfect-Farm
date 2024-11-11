using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{

    public PlantData plantData;
    public string color;
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
        if (color == "") color = gameObject.name.Split(" ")[0];
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
    }

    // Update is called once per frame
    void Update()
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
        if (currentGrowthStage == 2 && gameObject.name.Contains("Pepper"))
        {
            RandomizeColor();
        }
        if (currentGrowthStage == 3)
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



    void OnMouseUp()
    {
        if (isHarvestable && !hasBeenHarvested)
        {
            Debug.Log("Plant Harvested!");
            StartCoroutine(Harvest());
        }
        else if (!isHarvestable)
        {
            Debug.Log("Plant is not fully grown yet.");
        }
    }


    IEnumerator Harvest()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
        if (plot != null)
        {
            plot.ReEnablePlot();
        }
        //hasBeenHarvested = true;
        Debug.Log(color);
    }

    private void RandomizeColor()
    {
        if (plantData.growthStages.Length > 4)
        {
            Sprite temp = plantData.growthStages[3];
            int rand = Mathf.RoundToInt(Random.value * (plantData.growthStages.Length - 4));
            plantData.growthStages[3] = plantData.growthStages[rand + 3];
            plantData.growthStages[rand + 3] = temp;
            color = plantData.growthStages[3].name.Split(" ")[0];
        }
    }

}