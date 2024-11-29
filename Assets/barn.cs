using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barn : MonoBehaviour
{
    public GameObject inventory;
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

        inventory.SetActive(true);
       
    }
}
