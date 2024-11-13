using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour
{

    public GameObject teamSelectUI;
    private List<GameObject> selectedCats = new List<GameObject>();
    private int maxTeamSize = 3;
    public static ExpeditionManager instance;
    private Expedition currentExpedition;
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
        Debug.Log("Team selection UI opened by: " + expedition.name);
    }

    public void CatSelected(GameObject cat){

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
    }

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
