using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionManager : MonoBehaviour
{

    public GameObject teamSelectUI;
    private List<GameObject> selectedCats = new List<GameObject>();
    private int maxTeamSize = 3;
    public static ExpeditionManager instance;
    private Expedition currentExpedition;
    public List<Image> catImages;
    public Sprite defaultSprite;
    public int totalStrength = 0;
    public int totalSpeed = 0;
    public int totalDefense = 0;
    public Text strengthText;
    public Text speedText;
    public Text defenseText;
    // Start is called before the first frame update
    void Awake(){
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
   

    public void OpenTeamSelectUI(Expedition expedition){
        currentExpedition = expedition;
        totalStrength = 0;
        totalSpeed = 0;
        totalDefense = 0;
        strengthText.text = "Team Strength: " + totalStrength;
        speedText.text = "Team Speed: " + totalSpeed;
        defenseText.text = "Team Strength: " + totalDefense;
        selectedCats.Clear();
        teamSelectUI.SetActive(true);
        Toggle[] toggles = teamSelectUI.GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles) {
        toggle.isOn = false;
        }
        foreach(Image image in catImages){
                    if (image.sprite != defaultSprite){
                        image.sprite = defaultSprite;
                    }
                }
        Debug.Log("Team selection UI opened by: " + expedition.name);
    }

    public void CatToggled(bool isOn, GameObject cat, Sprite catSprite, Sprite catFace){
        if(isOn){
            if(selectedCats.Count < maxTeamSize){
                selectedCats.Add(cat);
                currentExpedition.CatFaces(isOn, catFace);
                foreach(Image image in catImages){
                    if (image.sprite == defaultSprite){
                        image.sprite = catSprite;
                        break;
                    }
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
            else{
                Debug.Log("Max team size is 3");
            }
        }

        else{
            if(selectedCats.Contains(cat)){
                currentExpedition.CatFaces(isOn, catFace);
                selectedCats.Remove(cat);
                foreach(Image image in catImages){
                    if (image.sprite == catSprite){
                        image.sprite = defaultSprite;
                        break;
                    }
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

    public void SendExpeditionManager(){
        if(selectedCats.Count == 0){
            Debug.Log("Must send at least 1 cat for expedition!");
        }

        

        if(currentExpedition != null){
            if(selectedCats.Count > 0){
                currentExpedition.SetSelectedTeam(selectedCats);
            
            if(currentExpedition.SendCatsonExpedition()){
                teamSelectUI.SetActive(false);
            }
            }
            
        
    }
    }

    public void CloseExpeditionMenu(){
        teamSelectUI.SetActive(false);
    }


}
