using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CatCheckbox : MonoBehaviour
{

    public GameObject cat;
    public TextMeshProUGUI color;
    private Toggle toggle;
    private Sprite catSprite, catFace;
    // Start is called before the first frame update
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
        string c = cat.GetComponent<Cat>().GetColor();
        catSprite = cat.GetComponent<SpriteRenderer>().sprite;
        catFace = Resources.Load<Sprite>("Sprites/Cats/Cat_Face/" + c);
    }

    private void OnValueChanged(bool isOn){
        ExpeditionManager.instance.CatToggled(isOn, cat, catSprite, catFace, transform.parent.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
