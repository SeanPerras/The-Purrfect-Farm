using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        //if(transform.position)
        //if (Input.GetMouseButtonDown(0))
        //    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //else if (Input.GetMouseButton(0))
        //{
        //    if (mousePos != Vector3.zero && Camera.main.ScreenToWorldPoint(Input.mousePosition) != mousePos)
        //        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0,0,10);
        //}
        //else if (Input.GetMouseButtonUp(0))
        //    mousePos = Vector3.zero;

    }
    private void OnMouseDown()
    {
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(mousePos);
    }
    private void OnMouseDrag()
    {
        //if (mousePos != Vector3.zero && Camera.main.ScreenToWorldPoint(Input.mousePosition) != mousePos)
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
    }
    private void OnMouseUp()
    {
        //mousePos = Vector3.zero;
    }
    private Vector3 WithinRange(Vector3 loc)
    {
        return loc;
    }
    public void SetColor(string c)
    {
        color = c;
        //#if UNITY_EDITOR
        //sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath + c + ".png");
        //#else 
        sprite = Resources.Load<Sprite>(spritePath + c);  // Make sure the sprite is in the Resources folder
        //#endif
        SpriteRenderer curr_Sprite = GetComponent<SpriteRenderer>();
        curr_Sprite.sprite = sprite;
        curr_Sprite.color = UnityEngine.ColorUtility.TryParseHtmlString(c, out Color co) ? co : Color.white;
        curr_Sprite.color = new Color(co.r * 40 + 40, co.g * 40 + 40, co.b * 40 + 40);
        name = c + " Cat";

        //Debug.Log(AssetDatabase.GetAssetPath(sprite));
    }
}
