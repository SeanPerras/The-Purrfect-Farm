using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionManager : MonoBehaviour
{
    public static ExpeditionManager instance;
    public GameObject teamSelectUI, catButtonPF;
    public Transform catButtons;
    public List<Image> catImages;
    public Sprite defaultSprite;
    public int totalStrength = 0;
    public int totalSpeed = 0;
    public int totalDefense = 0;
    public Text strengthText;
    public Text speedText;
    public Text defenseText;

    private List<GameObject> selectedCats = new(), selectedButtons = new();
    private readonly int maxTeamSize = 3;
    private Expedition currentExpedition;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        //List<Cat> cats = GameObject.Find("Cats").GetComponentsInChildren<Cat>().ToList();
        //PopulateCats(cats);
    }


    public void OpenTeamSelectUI(Expedition expedition)
    {
        currentExpedition = expedition;
        totalStrength = 0;
        totalSpeed = 0;
        totalDefense = 0;
        strengthText.text = "Team Strength: " + totalStrength;
        speedText.text = "Team Speed: " + totalSpeed;
        defenseText.text = "Team Strength: " + totalDefense;
        selectedCats.Clear();
        selectedButtons.Clear();
        teamSelectUI.SetActive(true);
        Toggle[] toggles = teamSelectUI.GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
            toggle.isOn = false;
        foreach (Image image in catImages)
            if (image.sprite != defaultSprite)
                image.sprite = defaultSprite;
        Debug.Log("Team selection UI opened by: " + expedition.name);
    }
    public void PopulateCats(List<Cat> cats)
    {
        int count = catButtons.childCount;
        Vector2 offset = new(162, -35);
        foreach (Cat cat in cats)
        {
            GameObject newBtn = Instantiate(catButtonPF, catButtons);
            newBtn.transform.localPosition = offset + new Vector2(0, -70*count);
            newBtn.name = cat.name;
            newBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = cat.name;
            newBtn.GetComponentInChildren<CatCheckbox>().cat = cat.gameObject;
            if (++count > 4) catButtons.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 70);
        }
    }

    public void CatToggled(bool isOn, GameObject cat, Sprite catSprite, Sprite catFace, GameObject button)
    {
        if (isOn)
        {
            if (selectedCats.Count < maxTeamSize)
            {
                selectedCats.Add(cat);
                selectedButtons.Add(button);
                //currentExpedition.CatFaces(isOn, catFace);
                foreach (Image image in catImages)
                    if (image.sprite == defaultSprite)
                    {
                        image.sprite = catSprite;
                        break;
                    }
                Debug.Log(cat.name + " added to the team.");

                Cat catComponent = cat.GetComponent<Cat>();
                totalStrength += catComponent.stats.strength;
                totalSpeed += catComponent.stats.speed;
                totalDefense += catComponent.stats.defense;

                strengthText.text = "Team Strength: " + totalStrength;
                speedText.text = "Team Speed: " + totalSpeed;
                defenseText.text = "Team Strength: " + totalDefense;
            }
            else
            {
                button.transform.Find("CheckBox").GetComponent<Toggle>().isOn = false;
                Debug.Log("Max team size is 3");
            }
        }

        else
        {
            if (selectedCats.Contains(cat))
            {
                currentExpedition.CatFaces(isOn, catFace);
                selectedCats.Remove(cat);
                selectedButtons.Remove(button);
                foreach (Image image in catImages)
                    if (image.sprite == catSprite)
                    {
                        image.sprite = defaultSprite;
                        break;
                    }
                Debug.Log(cat.name + " removed from the team");
                Cat catComponent = cat.GetComponent<Cat>();
                totalStrength -= catComponent.stats.strength;
                totalSpeed -= catComponent.stats.speed;
                totalDefense -= catComponent.stats.defense;

                strengthText.text = "Team Strength: " + totalStrength;
                speedText.text = "Team Speed: " + totalSpeed;
                defenseText.text = "Team Strength: " + totalDefense;
            }
        }
    }

    /*public void CatSelected(GameObject cat){

        if(selectedCats.Count >= maxTeamSize){
            Debug.Log("Max team size is 3");
            return;
        }
        else{
            selectedCats.Add(cat);
            Debug.Log(cat.name + " added to the team.");
        }
    }

    public void CatRemoved(){
        if(selectedCats.Count > 0){
            GameObject removeCat = selectedCats[selectedCats.Count - 1];
            selectedCats.RemoveAt(selectedCats.Count - 1);
            Debug.Log(removeCat.name + " removed from the team");

        }
    }*/

    public void SendExpeditionManager()
    {
        if (selectedCats.Count == 0)
        {
            Debug.Log("Must send at least 1 cat for expedition!");
        }
        if (currentExpedition != null)
        {
            if (selectedCats.Count > 0)
            {
                currentExpedition.SetSelectedTeam(selectedCats, selectedButtons);

                if (currentExpedition.SendCatsonExpedition())
                {
                    //currentExpedition.SetSelectedTeam(selectedCats, selectedButtons);
                    foreach (GameObject cat in selectedCats)
                        currentExpedition.CatFaces(true, cat.GetComponent<SpriteRenderer>().sprite);
                    CloseExpeditionMenu();
                }
            }
        }
    }

    public void CloseExpeditionMenu()
    {
        //if(!currentExpedition.inProgress) { currentExpedition.CatFaces(false, defaultSprite); }
        teamSelectUI.SetActive(false);
    }


}
