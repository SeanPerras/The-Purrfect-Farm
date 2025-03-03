using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barn : MonoBehaviour
{
    public GameObject inventoryUI;
    public Sprite openbarn;
    public Sprite closebarn;

    private Farm farm;
    void Start()
    {
        farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
    }
    void OnMouseDown()
    {
        if(!farm.IsAnyUIOpen())
        {
            Debug.Log("Barn clicked!");
            
            //if (TryGetComponent(out SpriteRenderer spriteRenderer))
            //{
            //    //spriteRenderer.sprite = Resources.Load<Sprite>("BarnOpen");
            //    spriteRenderer.sprite = openbarn;
            //}
            //else
            //{
            //    Debug.LogError("No SpriteRenderer component found on this GameObject!");
            //}

            farm.CloseOpenUIs(inventoryUI);
            inventoryUI.SetActive(true);
        }
       
    }
    public void Exit()
    {
        Debug.Log("barn closed!");

        //if (TryGetComponent(out SpriteRenderer spriteRenderer))
        //{
        //    //spriteRenderer.sprite = Resources.Load<Sprite>("BarnOpen");
        //    //spriteRenderer.sprite = closebarn;
        //}
        //else
        //{
        //    Debug.LogError("No SpriteRenderer component found on this GameObject!");
        //}

        inventoryUI.SetActive(false);
    }
}
