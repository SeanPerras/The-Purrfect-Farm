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
    // Start is called before the first frame update
    void Awake(){
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
   

    public void OpenTeamSelectUI(Expedition expedition){
        currentExpedition = expedition;
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

    public void CatToggled(bool isOn, GameObject cat, Sprite catSprite){
        if(isOn){
            if(selectedCats.Count < maxTeamSize){
                selectedCats.Add(cat);
                foreach(Image image in catImages){
                    if (image.sprite == defaultSprite){
                        image.sprite = catSprite;
                        break;
                    }
                }
                Debug.Log(cat.name + " added to the team.");
            }
            else{
                Debug.Log("Max team size is 3");
            }
        }

        else{
            if(selectedCats.Contains(cat)){
                selectedCats.Remove(cat);
                foreach(Image image in catImages){
                    if (image.sprite == catSprite){
                        image.sprite = defaultSprite;
                        break;
                    }
                }
                Debug.Log(cat.name + " removed from the team");
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


}
