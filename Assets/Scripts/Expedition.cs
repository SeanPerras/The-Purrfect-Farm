using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expedition : MonoBehaviour
{

    private List<GameObject> expeditionTeam = new List<GameObject>();


    void OnMouseUp(){
        ExpeditionManager.instance.OpenTeamSelectUI(this);
    }


    public void SetSelectedTeam(List<GameObject> team){
        expeditionTeam = new List<GameObject>(team);
    }

    public bool SendCatsonExpedition()
    {
        if (expeditionTeam.Count == 0)
        {
            Debug.Log("No cats selected for the expedition!");
            return false;
        }

        Debug.Log("Sending the following cats on an expedition:");
        foreach (GameObject cat in expeditionTeam)
        {
            Debug.Log(cat.name);
        }

        expeditionTeam.Clear();
        return true;
    }


}
