using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Cat : MonoBehaviour
{
    public static bool hasCat = false;
    private bool isSelectedCat = false;

    public Sprite sprite;
    public Vector3 catioPos;

    private string color = "White", spritePath;
    private int sOrder;
    private Vector3 mousePos;
    public CatStats stats;
    // Start is called before the first frame update
    void Awake()
    {
        spritePath = "Sprites/Cats/Cat_Sit/";
        //transform.position = WithinRange(catioPos);//<-This is for spawning the cats at the catio.
        //SetColor(color);
        transform.SetParent(GameObject.Find("Cats").transform);
        transform.position = WithinRange(catioPos);
        GetComponent<SpriteRenderer>().sortingOrder = sOrder;
    }

    // Update is called once per frame
    void Update()
    {
        //if(transform.position)
        //if (Input.GetMouseButtonDown(0))
        //    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), gameObject.transform.position) <= 2)
        {
            if (!hasCat)
            {
                hasCat = true;
                isSelectedCat = true;
            }
            if (isSelectedCat)
            {
                gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                float posY = gameObject.transform.position.y;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)Mathf.Round(posY * -.4f + 10f);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            hasCat = false;
            isSelectedCat = hasCat;
        }
        //    mousePos = Vector3.zero;

    }
    public Vector3 WithinRange(Vector3 loc)
    {
        //Vector3(-7.75, 21.5, 0)
        //-5.5,+1.5
        float count = transform.parent.childCount,
        minx = loc.x - 13.5f, miny = loc.y + 1.5f,
        xpos = minx + 1.5f * ((count % 6) + 2*(int)(count / 6)),
        ypos = miny + .75f * ((count % 6) - (int)(count / 6));
        sOrder = 2 - (int)count % 6;
        Debug.Log(xpos + "," + ypos + "," + sOrder);
        return new Vector3(xpos, ypos, 0);
    }
    public void SetColor(string c)
    {
        color = c.Replace("_", " ");
        //#if UNITY_EDITOR
        //sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath + c + ".png");
        //#else 
        sprite = Resources.Load<Sprite>(spritePath + color);  // Make sure the sprite is in the Resources folder
        stats = Resources.Load<CatStats>("CatData/" + c.Replace(" ", "_") + "CatData");
        //#endif
        SpriteRenderer curr_Sprite = GetComponent<SpriteRenderer>();
        curr_Sprite.sprite = sprite;
        //curr_Sprite.color = UnityEngine.ColorUtility.TryParseHtmlString(color, out Color co) ? co : Color.white;
        //float r = Mathf.Clamp(co.r + 40f/255f, 0, 1), g = Mathf.Clamp(co.g + 40f / 255f, 0, 1), b = Mathf.Clamp(co.b + 40f / 255f, 0, 1);
        //curr_Sprite.color = new Color(r, g, b);
        name = color + " Cat";

        //Debug.Log(AssetDatabase.GetAssetPath(sprite));
    }
    public string GetColor() { return color; }
}
