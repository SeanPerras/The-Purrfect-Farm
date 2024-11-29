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
    private int playerBalance = 1000; // starting balance
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
        UpdateBalanceUI();
        allLayerMask = Camera.main.cullingMask;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateBalanceUI();
    }
    private void UpdateBalanceUI()
    {
        balanceText.text = playerBalance.ToString();

    }
    public void PurchaseItem(GameObject itemToAdd)
    {
       
        

         Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (clickedButton == null)
        {
            Debug.LogError("clickedButton is null! Ensure the Button is properly assigned in the OnClick event.");
            return;
        }

        TMP_Text itemPriceText = clickedButton.transform.Find("price").GetComponent<TMP_Text>();
        if (itemPriceText == null)
        {
            Debug.LogError("PriceText GameObject is missing the TMP_Text component!");
            return;
        }
        int itemPrice = int.Parse(itemPriceText.text);
        

        if (playerBalance >= itemPrice)
        {
            playerBalance -= itemPrice; 
            UpdateBalanceUI();
            Debug.Log($"Item purchased for {itemPrice}! Remaining balance: {playerBalance}");
           
            itembought(itemToAdd);
        }
        else
        {
            Debug.Log("Not enough balance to purchase this item!");
        }
    }

    public void itembought(GameObject itemToAdd) {

        if (itemToAdd.name == "fence")
        {
            int count = int.Parse(fencecount.text);
            count++;
            fencecount.text = count.ToString();

            Debug.Log("A fence is being bought!");

            return;
        }
        if (itemToAdd.name == "windmill")
        {
            int count = int.Parse(windmillcount.text);
            count++;
            windmillcount.text = count.ToString();

            Debug.Log("A windmill is being bought!");

            return;
        }
        if (itemToAdd.name == "wheelbarrow")
        {
            int count = int.Parse(wheelbarrowcount.text);
            count++;
            wheelbarrowcount.text = count.ToString();

            Debug.Log("A wheelbarrow is being bought!");

            return;
        }
        if (itemToAdd.name == "catsule")
        {   
            int count = int.Parse(catsulecount.text);
            count++;
            catsulecount.text = count.ToString();

            Debug.Log("A catsule is being bought!");

            return;
        }
        if (itemToAdd.name == "tomato")
        {
            int count = int.Parse(tomatocount.text);
            count++;
            tomatocount.text = count.ToString();

            Debug.Log("A tomato is being bought!");

            return;
        }
        if (itemToAdd.name == "squash")
        {
            int count = int.Parse(squashcount.text);
            count++;
            squashcount.text = count.ToString();

            Debug.Log("A squash is being bought!");

            return;
        }
        if (itemToAdd.name == "blackberry")
        {
            int count = int.Parse(blackberrycount.text);
            count++;
            blackberrycount.text = count.ToString();

            Debug.Log("A blackberry is being bought!");

            return;
        }
        if (itemToAdd.name == "bellpepper")
        {
            int count = int.Parse(bellpeppercount.text);
            count++;
            bellpeppercount.text = count.ToString();

            Debug.Log("A red bellpepper is being bought!");

            return;
        }
        if (itemToAdd.name == "blueberry")
        {
            int count = int.Parse(blueberrycount.text);
            count++;
            blueberrycount.text = count.ToString();

            Debug.Log("A blueberry is being bought!");

            return;
        }
        if (itemToAdd.name == "raddish")
        {
            int count = int.Parse(raddishcount.text);
            count++;
            raddishcount.text = count.ToString();

            Debug.Log("A raddish is being bought!");

            return;
        }



    }
   
   
}
