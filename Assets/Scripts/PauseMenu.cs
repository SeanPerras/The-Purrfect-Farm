using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public Slider musicVolume, sfxVolume;
    //public GameObject pauseButton;
    public static bool GameIsPaused = false;
    private void Start()
    {
        musicVolume.value = PlayerPrefs.GetFloat("Music", 1);
        sfxVolume.value = PlayerPrefs.GetFloat("SFX", 1);
    }
    void Update()
    {
        PauseEscape();
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        //pauseButton.SetActive(false);
        Time.timeScale = 0;
        GameIsPaused = true;
    }
    public void PauseEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Escape key pressed");
            if (GameIsPaused) Resume();
            else Pause();
        }
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }
    public void Resume()
    {
        StartCoroutine(Farm.DelayMenu(pauseMenu));//To prevent creating plots while clicking menu.
        //pauseMenu.SetActive(false);
        //pauseButton.SetActive(true);
        Time.timeScale = 1;
        GameIsPaused = false;
    }
    public void Save()
    {
        GameManager.instance.SaveGame();
    }

    public IEnumerator ExitGame()
    {
        yield return new WaitUntil(() => GameManager.instance.IsConfirmed());
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();  
        #endif
    }
}
