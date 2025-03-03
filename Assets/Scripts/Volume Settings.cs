using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    private VideoPlayer player;
    //[SerializeField] private Slider musicSlider;
    // Start is called before the first frame update
    void Start()
    {
        try { GameObject.Find("Farm Main Menu").TryGetComponent(out player); }
        catch { player = null; }
    }
    public void SetMusicVolume(Slider slider)
    {
        //float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(slider.value)*20);
    }
    public void SetSFXVolume(Slider slider)
    {
        if(player) player.SetDirectAudioVolume(0, slider.value);
        myMixer.SetFloat("sfx", Mathf.Log10(slider.value * .5f)*20);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
