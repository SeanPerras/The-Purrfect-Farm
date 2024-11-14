using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{

    private List<GameObject> expeditionTeam = new List<GameObject>();
    [SerializeField] private Text timerText;
    public ExpeditionData expeditionData;
    private bool inProgress = false;
    private bool isCompleted = false;
    private float expeditionTimer = 0f;


    void OnMouseUp(){
        if(!inProgress){
            timerText.text = "";
        ExpeditionManager.instance.OpenTeamSelectUI(this);
        }
        else if(isCompleted){
            ClaimRewards();
        }
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

        //expeditionTeam.Clear();
        timerText.text = "";
        StartCoroutine(StartExpeditionTimer());
        inProgress = true;
        return true;
    }


    private IEnumerator StartExpeditionTimer(){
        expeditionTimer = expeditionData.expeditionTime;
        while(expeditionTimer > 0){
            yield return new WaitForSeconds(1f);
            expeditionTimer -= 1f;
            UpdateVisualTimer();
            Debug.Log("Time remaining: " + expeditionTimer + " seconds");
        }

        Debug.Log("Expedition complete! Ready to claim.");
        inProgress = false;
        //UpdateVisualTimer();
        isCompleted = true;
        UpdateVisualTimer();
    }

    private void UpdateVisualTimer(){
        if(isCompleted){
            timerText.text = "Expedition Complete!";
        }
        else{
            timerText.text = "Time Left: " + Mathf.CeilToInt(expeditionTimer) + "s";
        }
    }


    private void ClaimRewards(){
        if(!isCompleted){
            Debug.Log("Expedition not done!");
            return;
        }
        timerText.text = "";
        expeditionTeam.Clear();
        isCompleted = false;
        Debug.Log("Expedition finished, team returned!");
    }


}
