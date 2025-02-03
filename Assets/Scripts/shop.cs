using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopUI;

    private Cat selectedCat = null;
    private void Update()
    {
        //if(!shopUI.activeSelf && Input.)

    }
    private void OnMouseDown()
    {
        shopUI.SetActive(true);
        selectedCat = null;
    }
    public void OnMouseOver()
    {
        //Debug.Log("Mouse is Over Shop.");
        if (Input.GetMouseButton(0) && Cat.hasCat)
            selectedCat = Cat.selectedCat;
        if (Input.GetMouseButtonUp(0) && selectedCat)
        {
            GameManager.instance.ObjectToConfirm(selectedCat.gameObject);
            GameManager.instance.WaitForConfirmation("Cat.Sell");
        }
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
    }
   public void CloseShop(GameObject shopUI)
    {
        StartCoroutine(Farm.DelayMenu(shopUI));
    }
   
}
