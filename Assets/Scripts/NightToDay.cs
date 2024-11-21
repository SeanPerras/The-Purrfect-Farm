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
        SceneManager.LoadScene("Home");

    }


    public void ToExpeditions(){
        GameManager.SaveJsonData(GameManager.instance);
        SceneManager.LoadScene("Expedition Map");

    }
}
