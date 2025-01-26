using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    //[SerializeField] private Slider musicSlider;
    // Start is called before the first frame update
    public void SetMusicVolume(Slider slider){
        //float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(slider.value)*20);
    }
    public void SetSFXVolume(Slider slider)
    {
        myMixer.SetFloat("sfx", Mathf.Log10(slider.value * .5f)*20);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
