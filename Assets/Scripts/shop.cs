using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopUI;

    private Cat selectedCat = null;
    private Farm farm;
    private void Start()
    {
        farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
    }
    private void OnMouseDown()
    {
        if (!farm.IsAnyUIOpen())
        {
            Debug.Log("Shop clicked!");

            farm.CloseOpenUIs(shopUI);
            shopUI.SetActive(true);
        }
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
        int itemPrice = int.Parse(itemToAdd.transform.Find("PriceTag").Find("Price").GetComponent<TMP_Text>().text);
        if (GameManager.CheckCost(itemPrice))
        {
            GameManager.instance.RemoveCoin(itemPrice);
            Debug.Log($"Item purchased for {itemPrice}! Remaining balance: {GameManager.instance.GetCurrency()}");
            Itembought(itemToAdd);
        }
        else Debug.Log("Not enough balance to purchase this item!");
    }
    public void Itembought(GameObject itemToAdd)
    {
        int value;
        string priceText = itemToAdd.transform.Find("PriceTag").GetComponentInChildren<TMP_Text>().text;
        if (itemToAdd.name.Contains("Seed")) value = int.Parse(priceText);
        else value = int.Parse(priceText)/2;
        
        GameManager.instance.UpdateInventory(itemToAdd.name, value);
    }
   public void CloseShop(GameObject shopUI)
    {
        StartCoroutine(Farm.DelayMenu(shopUI));
    }
   
}
