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
    public Sprite sprite;
    public Vector3 catioPos;

    private string color = "White";
    private string spritePath;
    private Vector3 mousePos;
    public CatStats stats;
    // Start is called before the first frame update
    void Awake()
    {
        //#if UNITY_EDITOR
        //spritePath = AssetDatabase.GetAssetPath(sprite);
        //spritePath = spritePath[..(spritePath.IndexOf(color))];
        //#endif
        spritePath = "Sprites/Cats/Cat_Sit/";
        //transform.position = WithinRange(catioPos);//<-This is for spawning the cats at the catio.
        //SetColor(color);
        transform.SetParent(GameObject.Find("Cats").transform);
    }

    // Update is called once per frame
    void Update()
    {
        //if(transform.position)
        //if (Input.GetMouseButtonDown(0))
        //    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition),transform.position) <= 4)
        {
            //if (mousePos != Vector3.zero && Camera.main.ScreenToWorldPoint(Input.mousePosition) != mousePos)
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        }
        //else if (Input.GetMouseButtonUp(0))
        //    mousePos = Vector3.zero;

    }
    private Vector3 WithinRange(Vector3 loc)
    {
        return loc; //For now.
    }
    public void SetColor(string c)
    {
        color = c;
        //#if UNITY_EDITOR
        //sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath + c + ".png");
        //#else 
        sprite = Resources.Load<Sprite>(spritePath + c);  // Make sure the sprite is in the Resources folder
        stats = Resources.Load<CatStats>("CatData/" + c + "CatData");
        //#endif
        SpriteRenderer curr_Sprite = GetComponent<SpriteRenderer>();
        curr_Sprite.sprite = sprite;
        curr_Sprite.color = UnityEngine.ColorUtility.TryParseHtmlString(c, out Color co) ? co : Color.white;
        float r = Mathf.Clamp(co.r + 40f/255f, 0, 1), g = Mathf.Clamp(co.g + 40f / 255f, 0, 1), b = Mathf.Clamp(co.b + 40f / 255f, 0, 1);
        curr_Sprite.color = new Color(r, g, b);
        name = c + " Cat";

        //Debug.Log(AssetDatabase.GetAssetPath(sprite));
    }
}
