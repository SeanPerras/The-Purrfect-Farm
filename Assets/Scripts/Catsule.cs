using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Catsule : MonoBehaviour
{
    public GameObject catPrefab;
    public Sprite catsuleSprite;

    private SpriteRenderer sprite;
    private Plot currentPlot;
    private float timer = 1000f;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = catsuleSprite;
        //List<Collider2D> lc = Physics2D.OverlapCircleAll(transform.position, .1f).ToList();
        //GameObject pc = lc.Find(x => x.gameObject.CompareTag("Plot")).gameObject;
        currentPlot = Physics2D.OverlapCircleAll(transform.position, .1f).ToList().Find(x => x.gameObject.CompareTag("Plot")).gameObject.GetComponent<Plot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0)  CheckTimer();
        
    }
    private void OnMouseUp()
    {
        if(timer <= 0) StartCoroutine(TimesUp());
    }
    private void CheckTimer()
    {
        if (timer == 0) transform.GetChild(0).GetComponent<TextMeshPro>().text = "Ready!";
        else transform.GetChild(0).GetComponent<TextMeshPro>().text = timer--.ToString();
    }

    IEnumerator TimesUp()
    {
        yield return new WaitForSeconds(.25f);
        GameObject cat = Instantiate(catPrefab, transform.position, catPrefab.transform.rotation);
        Plot[] plots = currentPlot.GetAdjPlots().Where(g => g != null).Select(go => go.GetComponent<Plot>()).ToArray();
        List<string> colors = plots.Where(p => p.plant != null).Select(pl => pl.plant.GetComponent<Plant>().color).ToList();
        string col; //= GameObject.Find("Farm (9x9)").GetComponent<Farm>().Colors.First(kvp => kvp.Value.SequenceEqual(colors) || ContainsAll(colors, kvp.Value)).Key;
        try{
            col = GameObject.Find("Farm (9x9)").GetComponent<Farm>().Colors.First(kvp => kvp.Value.SequenceEqual(colors) || ContainsAll(colors, kvp.Value)).Key;
        }
        catch{col = "White"; }
        cat.GetComponent<Cat>().SetColor(col);
        Destroy(gameObject);
    }
    private bool ContainsAll(List<string> l1, List<string> l2)
    {
        bool ret = false;
        foreach (string s in l2)
            if (l1.Contains(s))
                ret = true;
            else return false;
        return ret;
    }
}
