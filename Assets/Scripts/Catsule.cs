using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Catsule : MonoBehaviour
{
    public GameObject catPrefab;
    public Sprite catsuleSprite;

    private SpriteRenderer sprite;
    private Plot currentPlot;
    private float timer = 100f;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = catsuleSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0)  CheckTimer();
        
    }
    private void OnMouseUp()
    {
        TimesUp();
    }
    private void CheckTimer()
    {
        if (timer == 0) transform.GetChild(0).GetComponent<TextMeshPro>().text = "Ready!";
        else
        {
            transform.GetChild(0).GetComponent<TextMeshPro>().text = timer--.ToString();
            //timer--;
        }
    }

    private void TimesUp()
    {
        GameObject cat = Instantiate(catPrefab, transform.position, catPrefab.transform.rotation);
        cat.GetComponent<Cat>().SetColor("Black");
        Destroy(gameObject);
    }
}
