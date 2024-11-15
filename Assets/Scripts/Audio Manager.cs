using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;

    public AudioClip main_menu;
    public AudioClip farm_day;
    public AudioClip expedition;
    //public AudioClip virusDeath;
    //public AudioClip bossDeath;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        
        PlayMusicForScene(SceneManager.GetActiveScene().buildIndex);
    }
    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.buildIndex);
    }

    private void PlayMusicForScene(int sceneIndex)
    {
        if (musicSource == null)
    {
        musicSource = gameObject.AddComponent<AudioSource>();
    }
        switch (sceneIndex)
        {
            case 0:
                // Play main menu music
                musicSource.clip = main_menu;
                musicSource.loop = true;
                break;
            case 1:
                // Play main menu music
                musicSource.clip = farm_day;
                musicSource.loop = true;
                break;

            case 2:
                musicSource.clip = expedition;
                musicSource.loop = true;
                break;
            default:
                break;
        }

        musicSource.Play();
    }

    
}
