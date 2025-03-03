using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider musicVolume, sfxVolume;
    private void Start()
    {
        musicVolume.value = PlayerPrefs.GetFloat("Music", 1);
        sfxVolume.value = PlayerPrefs.GetFloat("SFX", 1);
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Home");
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();  
        #endif
    }
    public void Options(){
        
    }
}
