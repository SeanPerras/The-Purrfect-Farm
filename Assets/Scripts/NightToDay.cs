using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightToDay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToFarm(){
        Expedition currentExpedition = FindObjectOfType<Expedition>();
        if(ExpeditionSLManager.instance != null)
        ExpeditionSLManager.SaveExpoJsonData(ExpeditionSLManager.instance, currentExpedition);
        SceneManager.LoadScene("Home");

    }


    public void ToExpeditions(){
        if(GameManager.instance != null){
        GameManager.SaveJsonData(GameManager.instance);
        SceneManager.LoadScene("Expedition Map");
        }

    }
}
