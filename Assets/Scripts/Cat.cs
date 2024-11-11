using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Cat : MonoBehaviour
{
    public Sprite sprite;

    private string color = "White";
    private string spritePath;
    // Start is called before the first frame update
    void Awake()
    {
        spritePath = AssetDatabase.GetAssetPath(sprite);
        spritePath = spritePath[..(spritePath.IndexOf(color))];
        SetColor(color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Spawn()
    {
        
    }
    public void SetColor(string c)
    {
        color = c;
        sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath + c + ".png");
        GetComponent<SpriteRenderer>().sprite = sprite;
        Debug.Log(AssetDatabase.GetAssetPath(sprite));
    }
}
