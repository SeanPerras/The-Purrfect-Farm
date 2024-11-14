using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatCheckbox : MonoBehaviour
{

    public GameObject cat;
    private Toggle toggle;
    public Sprite catSprite;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
        
    }

    private void OnValueChanged(bool isOn){
        ExpeditionManager.instance.CatToggled(isOn, cat, catSprite);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
