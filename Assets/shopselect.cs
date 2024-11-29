using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopselect : MonoBehaviour
{
    public GameObject shopmenu;
    public LayerMask shopLayer;
    public GameObject farm;
    public GameObject plots;
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
        //Camera.main.cullingMask = shopLayer;
        //farm.layer = LayerMask.NameToLayer("Ignore Raycast");
        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        SetLayerRecursively(farm, ignoreRaycastLayer);
        SetLayerRecursively(plots, ignoreRaycastLayer);

    }
    public void exit() {
        shopmenu.SetActive(false);
       
        int landLayer = LayerMask.NameToLayer("landLayer");
        SetLayerRecursively(farm, landLayer);
        farm.layer = LayerMask.NameToLayer("Default");

        int plotLayer = LayerMask.NameToLayer("plotLayer");
        SetLayerRecursively(plots, landLayer);
        plots.layer = LayerMask.NameToLayer("Default");

    }
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
