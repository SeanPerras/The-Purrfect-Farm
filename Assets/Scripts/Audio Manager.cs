using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;

    public AudioClip main_menu;
    //public AudioClip virusDeath;
    //public AudioClip bossDeath;

    private void Start()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;

        
        PlayMusicForScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.buildIndex);
    }

    private void PlayMusicForScene(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 0:
                // Play main menu music
                musicSource.clip = main_menu;
                musicSource.loop = true;
                break;
            default:
                break;
        }

        musicSource.Play();
    }

    
}
