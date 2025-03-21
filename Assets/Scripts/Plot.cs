using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Plot : MonoBehaviour
{
    private Plant plant;
    private Catsule catsule;
    private Farm farm;
    private List<Vector2> positions;
    private readonly GameObject[] adjPlots = new GameObject[4];
    private Collider2D plotCollider;
    private GameObject oldLand, rightClickMenu;
    //private int totalPlants = 0;
    //public GameObject plantPrefab;
    // Start is called before the first frame update
    void Start()
    {
        name = name.Remove(3, 7);
        //name = name[..name.IndexOf("(Clone)")];
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
                    go.name.Contains("Plant") ? go.GetComponent<Plant>().GetPlotReference().gameObject :
                    go.name.Contains("Catsule") ? go.GetComponent<Catsule>().GetPlotReference().gameObject :
                    null;
                if(adjPlots[index]) adjPlots[index].GetComponent<Plot>().AddPlotRef(gameObject, (index + 2) % 4);
            }
            index++;
        }
        plotCollider = GetComponent<Collider2D>();
        farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
        rightClickMenu = farm.sellMenu;
    }
    //private void OnMouseEnter() { transform.Find("Outline").gameObject.SetActive(true); }
    //private void OnMouseExit() { transform.Find("Outline").gameObject.SetActive(false); }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1) && plant == null && !farm.IsAnyUIOpen())
        {
            rightClickMenu.transform.position = transform.position + new Vector3(0, 1, 0);
            rightClickMenu.GetComponent<TextMeshPro>().text = name;
            rightClickMenu.transform.Find("SellButton").GetComponentInChildren<TMP_Text>().text = "Sell: 5";
            rightClickMenu.SetActive(true);
            rightClickMenu.transform.GetChild(0).gameObject.SetActive(true);
            UnityEvent click = rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick;
            click.RemoveAllListeners();
            click.AddListener(() => GameManager.instance.ObjectToConfirm(gameObject));
            click.AddListener(() => GameManager.instance.WaitForConfirmation("Plot.Sell"));
        }
    }
    private void OnMouseUp()
    {
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.ObjectToConfirm(gameObject));
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.WaitForConfirmation("Plot.Sell"));
    }

    public void AddPlotRef(GameObject go, int ind) { adjPlots[ind] = go; }
    public Plant Plant(PlantData plantData)
    {
        plant = Instantiate(plantData.plantPrefab, transform.position + new Vector3(0, 1, 0),
                            transform.rotation, gameObject.transform).GetComponent<Plant>();
        plant.transform.localScale /= 10;
        plant.SetPlantData(plantData);
        plant.SetPlotReference(this);
        plant.gameObject.name = plantData.plantPrefab.name;
        plant.value = plantData.value;
        plant.sOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        //plant.GetComponent<SpriteRenderer>().sortingOrder = 20;//temp fix
        return plant;
    }
    public Catsule Plant(string color = "White")
    {
        GameObject prefab = GameManager.instance.catsulePrefabs.Find(c => c.GetComponent<Catsule>().color == color);
        Catsule ct = Instantiate(prefab, transform.position, transform.rotation, gameObject.transform).GetComponent<Catsule>();
        ct.transform.localScale /= 10;
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
    public void Sell()
    {
        //yield return new WaitUntil(() => GameManager.instance.IsConfirmed());
        if(plant) Destroy(plant);
        oldLand.SetActive(true);
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.ObjectToConfirm(gameObject));
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.WaitForConfirmation("Plot.Sell"));
        rightClickMenu.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(Farm.DelayMenu(rightClickMenu));//.SetActive(false);
        GameManager.instance.AddCoin(5);
        gameObject.SetActive(false);
        Destroy(gameObject, .25f);
    }



    public Plant GetPlant() { return plant; }
    public Catsule GetCatsule() { return catsule; }
}
