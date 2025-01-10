using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class NightToDay : MonoBehaviour
{
    public GameObject FARM, EXPO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToFarm()
    {
        Expedition[] allExpeditions = FindObjectsOfType<Expedition>();
        if (ExpeditionSLManager.instance != null)
            ExpeditionSLManager.SaveExpoJsonData(ExpeditionSLManager.instance, allExpeditions.ToList());
        SceneManager.LoadScene("Home");
    }


    public void ToExpeditions()
    {
        //if (GameManager.instance != null)
        //{
        //    GameManager.SaveJsonData(GameManager.instance);
        //    //ExpeditionSLManager.LoadExpoJsonData(ExpeditionSLManager.instance);
        //    SceneManager.LoadScene("Expedition Map");
        //}
        FARM.SetActive(false);
        EXPO.SetActive(true);
    }
}
