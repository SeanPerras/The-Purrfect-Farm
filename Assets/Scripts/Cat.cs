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
        #if UNITY_EDITOR
        spritePath = AssetDatabase.GetAssetPath(sprite);
        spritePath = spritePath[..(spritePath.IndexOf(color))];
        #endif
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
        #if UNITY_EDITOR
        sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath + c + ".png");
        #else 
        sprite = Resources.Load<Sprite>("Sprites/Cats/Cat_Sit/" + c);  // Make sure the sprite is in the Resources folder
        #endif
        GetComponent<SpriteRenderer>().sprite = sprite;
        //Debug.Log(AssetDatabase.GetAssetPath(sprite));
    }
}
