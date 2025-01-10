using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class shop : MonoBehaviour
{
   // public GameObject shopmenu;
    public LayerMask shopLayer;
    private int allLayerMask;
    
    //public Text catsulecount='';
    public TMP_Text balanceText; 
    private int playerBalance; // starting balance
    public TMP_Text fencecount;
    public TMP_Text windmillcount;
    public TMP_Text wheelbarrowcount;
    public TMP_Text raddishcount;
    public TMP_Text catsulecount;
    public TMP_Text blueberrycount;
    public TMP_Text tomatocount;
    public TMP_Text blackberrycount;
    public TMP_Text squashcount;
    public TMP_Text bellpeppercount;


    // Start is called before the first frame update
    void Start()
    {
        playerBalance = GameManager.instance.GetCurrency();
        //UpdateBalanceUI();
        allLayerMask = Camera.main.cullingMask;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PurchaseItem(GameObject itemToAdd)
    {
        if (!EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out var clickedButton))
        {
            Debug.LogError("clickedButton is null! Ensure the Button is properly assigned in the OnClick event.");
            return;
        }
        if (!clickedButton.transform.parent.Find("pricetag").GetChild(0).TryGetComponent<TMP_Text>(out var itemPriceText))
        {
            Debug.LogError("PriceText GameObject is missing the TMP_Text component!");
            return;
        }
        int itemPrice = int.Parse(itemPriceText.text);
        if (GameManager.instance.GetCurrency() >= itemPrice)
        {
            GameManager.instance.RemoveCoin(itemPrice);

            Debug.Log($"Item purchased for {itemPrice}! Remaining balance: {GameManager.instance.GetCurrency()}");
           
            Itembought(itemToAdd);
        }
        else
        {
            Debug.Log("Not enough balance to purchase this item!");
        }
    }
    public void Itembought(GameObject itemToAdd)
    {
        string variety = "", category = "";
        if(itemToAdd.name.Contains("Seed"))
        {
            variety = itemToAdd.name[..itemToAdd.name.IndexOf(" Seed")];
            category = "Seed";
        }
        else if (itemToAdd.name.Contains("Catsule"))
        {
            variety = itemToAdd.name[..itemToAdd.name.IndexOf(" Catsule")];
            category = "Catsule";
        }
        else if (itemToAdd.name.Contains("Decoration"))
        {
            variety = itemToAdd.name[..itemToAdd.name.IndexOf(" Decoration")];
            category = "Decor";
        }
        GameManager.instance.UpdateInventory(variety, category);
        //if (itemToAdd.name == "fence")
        //{
        //    int count = int.Parse(fencecount.text);
        //    count++;
        //    fencecount.text = count.ToString();

        //    Debug.Log("A fence is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "windmill")
        //{
        //    int count = int.Parse(windmillcount.text);
        //    count++;
        //    windmillcount.text = count.ToString();

        //    Debug.Log("A windmill is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "wheelbarrow")
        //{
        //    int count = int.Parse(wheelbarrowcount.text);
        //    count++;
        //    wheelbarrowcount.text = count.ToString();

        //    Debug.Log("A wheelbarrow is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "catsule")
        //{   
        //    int count = int.Parse(catsulecount.text);
        //    count++;
        //    catsulecount.text = count.ToString();

        //    Debug.Log("A catsule is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "tomato")
        //{
        //    int count = int.Parse(tomatocount.text);
        //    count++;
        //    tomatocount.text = count.ToString();

        //    Debug.Log("A tomato is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "squash")
        //{
        //    int count = int.Parse(squashcount.text);
        //    count++;
        //    squashcount.text = count.ToString();

        //    Debug.Log("A squash is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "blackberry")
        //{
        //    int count = int.Parse(blackberrycount.text);
        //    count++;
        //    blackberrycount.text = count.ToString();

        //    Debug.Log("A blackberry is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "bellpepper")
        //{
        //    int count = int.Parse(bellpeppercount.text);
        //    count++;
        //    bellpeppercount.text = count.ToString();

        //    Debug.Log("A red bellpepper is being bought!");

        //    return;
        //}
        //if (itemToAdd.name == "Blueberry")
        //{
        //    int count = int.Parse(blueberrycount.text);
        //    count++;
        //    blueberrycount.text = count.ToString();

        //    Debug.Log("A blueberry is being bought!");


        //    return;
        //}
        //if (itemToAdd.name == "raddish")
        //{
        //    int count = int.Parse(raddishcount.text);
        //    count++;
        //    raddishcount.text = count.ToString();

        //    Debug.Log("A raddish is being bought!");

        //    return;
        //}



    }
   public void CloseShop(GameObject shopUI)
    {
        StartCoroutine(Farm.DelayMenu(shopUI));
    }
   
}
