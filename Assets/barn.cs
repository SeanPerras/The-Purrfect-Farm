using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barn : MonoBehaviour
{
    public GameObject inventory;
    public UnityEngine.Sprite openbarn;
    public UnityEngine.Sprite closebarn;

    private Farm farm;
    // Start is called before the first frame update
    void Start()
    {
        farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMouseDown()
    {
        if(!farm.IsAnyUIOpen())
        {
            Debug.Log("barn clicked!");
            
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
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
       
    }
    public void exit()
    {
        if (!farm.IsAnyUIOpen())
        {
            Debug.Log("barn closed!");
            
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
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
}
