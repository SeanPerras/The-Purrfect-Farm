using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopselect : MonoBehaviour
{
    public GameObject shopmenu;
    public LayerMask shopLayer;
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

        Debug.Log("shop clicked!");

        shopmenu.SetActive(true);
        Camera.main.cullingMask = shopLayer;
    }
}
