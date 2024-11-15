using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Catsule : MonoBehaviour
{
    public GameObject catPrefab;
    public Sprite catsuleSprite;
    public string color = "White";

    private SpriteRenderer sprite;
    private Plot currentPlot;
    private float timer = 15f;
    private GameObject timerText;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = catsuleSprite;
        timerText = transform.GetChild(0).gameObject;
        timerText.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0.0000f)  CheckTimer();
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
        Debug.Log("Time's Up!");
        GameObject cat = Instantiate(catPrefab, transform.position, catPrefab.transform.rotation);
        Plot[] plots = currentPlot.GetAdjPlots().Where(g => g != null).Select(go => go.GetComponent<Plot>()).ToArray();
        List<string> colors = plots.Select(p => p.GetColor()).ToList();
        string col;
        try{
            List<string> debug = colors;
            col = GameObject.Find("Farm (9x9)").GetComponent<Farm>().Colors.First(kvp => kvp.Value.SequenceEqual(colors) || ContainsAll(colors, kvp.Value)).Key;
        }
        catch{ col = "White"; }
        cat.GetComponent<Cat>().SetColor(col);
        Debug.Log(string.Join("|", colors));
        currentPlot.ReEnablePlot();
        Destroy(gameObject);
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
        List<string> temp = (from item in l1 select item[..]).ToList();
        foreach (string s in l2)
            if (temp.Contains(s))
            {
                ret = true;
                temp.Remove(s);
            }
            else return false;
        return ret;
    }
}
