using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barn : MonoBehaviour
{
    public GameObject inventory;
    public UnityEngine.Sprite openbarn;
    public UnityEngine.Sprite closebarn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDown()
    {

        Debug.Log("barn clicked!");
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            
            //spriteRenderer.sprite = Resources.Load<Sprite>("BarnOpen");
            spriteRenderer.sprite = openbarn;



        }
        else
        {
            Debug.LogError("No SpriteRenderer component found on this GameObject!");
        }

        inventory.SetActive(true);
       
    }
    public void exit()
    {

        Debug.Log("barn closed!");
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {

            //spriteRenderer.sprite = Resources.Load<Sprite>("BarnOpen");
            spriteRenderer.sprite = closebarn;



        }
        else
        {
            Debug.LogError("No SpriteRenderer component found on this GameObject!");
        }

        inventory.SetActive(false);

    }
}
