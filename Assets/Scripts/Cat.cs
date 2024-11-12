using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Cat : MonoBehaviour
{
    public Sprite sprite;
    public Vector3 catio;

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
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mousePos);
    }
    private void OnMouseDrag()
    {
        if (mousePos != Vector3.zero && Camera.main.ScreenToWorldPoint(Input.mousePosition) != mousePos)
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
    }
    private void OnMouseUp()
    {
        mousePos = Vector3.zero;
    }
    public void SetColor(string c)
    {
        color = c;
        //#if UNITY_EDITOR
        //sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath + c + ".png");
        //#else 
        sprite = Resources.Load<Sprite>(spritePath + c);  // Make sure the sprite is in the Resources folder
        //#endif
        GetComponent<SpriteRenderer>().sprite = sprite;
        //Debug.Log(AssetDatabase.GetAssetPath(sprite));
    }
}
