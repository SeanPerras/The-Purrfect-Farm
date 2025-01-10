using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Color outlineColor = Color.yellow;
    private GameObject outlineObj;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        outlineObj = new GameObject("Outline");
        outlineObj.transform.parent = transform;
        outlineObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        outlineObj.transform.localScale = Vector3.one * 1.05f; 
        var outlineSpriteRenderer = outlineObj.AddComponent<SpriteRenderer>();
        outlineSpriteRenderer.sprite = spriteRenderer.sprite; 
        outlineSpriteRenderer.material = new Material(Shader.Find("Sprites/Default")); 
        outlineSpriteRenderer.color = outlineColor; 

        outlineObj.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (outlineObj != null)
        {
            outlineObj.SetActive(true); 
        }
    }

    void OnMouseExit()
    {
        if (outlineObj != null)
        {
            outlineObj.SetActive(false); 
        }
    }
}
