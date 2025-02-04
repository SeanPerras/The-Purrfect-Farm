using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Catsule : MonoBehaviour
{
    public Sprite catsuleSprite;
    public string color = "White";

    private SpriteRenderer sprite;
    private Plot currentPlot;
    private float timer = 15f;
    private GameObject timerText;
    private Farm farm;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = catsuleSprite;
        timerText = transform.GetChild(0).gameObject;
        //timerText.GetComponent<Renderer>().enabled = false;
        name = name[..name.IndexOf("(Clone)")];
        farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0.0000f && gameObject.activeSelf)  CheckTimer();
    }
    private void OnMouseUp()
    {
        Debug.Log("Catsule MouseUp!");
        if (Mathf.Round(timer) <= 0) StartCoroutine(TimesUp());
    }
    private void OnMouseEnter()
    {
        if (timerText.GetComponent<TextMeshPro>().text != "Ready!")
            timerText.GetComponent<Renderer>().enabled = true;
    }
    private void OnMouseExit()
    {
        if (timerText.GetComponent<TextMeshPro>().text != "Ready!")
            timerText.GetComponent<Renderer>().enabled = false;
    }
    private void CheckTimer()
    {
        if (Mathf.Round(timer) <= 0)
        {
            timerText.GetComponent<TextMeshPro>().text = "Ready!";
            timerText.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            timerText.GetComponent<TextMeshPro>().text = Mathf.Round(timer).ToString();
            timer -= Time.deltaTime;
        }
    }

    IEnumerator TimesUp()
    {
        yield return new WaitForSeconds(.25f);
        GameObject cat = Instantiate(GameManager.instance.catPrefab, transform.position, GameManager.instance.catPrefab.transform.rotation);
        Debug.Log("Time's Up!");
        Plot[] plots = currentPlot.GetAdjPlots().Where(g => g != null).Select(go => go.GetComponent<Plot>()).ToArray();
        List<string> adjColors = plots.Select(p => p.GetColor()).ToList();
        string col;
        List<KeyValuePair<string, List<string>>> debug = Farm.Colors.Where(
            kvp => kvp.Value.SequenceEqual(adjColors) ||
            ContainsAll(adjColors, kvp.Value)).ToList();
        col = BestPick(debug);
        cat.GetComponent<Cat>().SetColor(col);
        Debug.Log(string.Join("|", adjColors));
        currentPlot.ReEnablePlot();
        Destroy(gameObject);
        farm.UpdateIcons(col + " Cat");
    }
    private string BestPick(List<KeyValuePair<string, List<string>>> colors)
    {
        colors = colors.OrderByDescending(kvp => kvp.Value.Count).ToList();
        int most = 0;
        string ret = "White";
        foreach(var c in colors)
        {
            if (c.Value.Distinct().Count() > most)
            {
                most = c.Value.Distinct().Count();
                ret = c.Key;
            }
            else continue;
        }
        return ret;
    }
    public void SetPlotReference(Plot assignedPlot)
    {
        currentPlot = assignedPlot;
        currentPlot.GetComponent<Collider2D>().enabled = false;
    }
    public Plot GetPlotReference() { return currentPlot; }
    private bool ContainsAll(List<string> l1, List<string> l2)
    {
        bool ret = false;
        //List<string> temp = (from item in l1 select item[..]).ToList();
        List<string> temp = new(l1);
        foreach (string s in l2)
            if (temp.Contains(s))
            {
                ret = true;
                temp.Remove(s);
            }
            else return false;
        return ret;
    }



    public float Timer
    {
        get { return timer; }
        set { timer = value; }
    }
    public float GrowthStage
    {
        get { return 0; }
        set { }
    }
}
