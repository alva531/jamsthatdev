using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioMixerController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        // Si no existen los PlayerPrefs, los inicializa a 0.5
        if (!PlayerPrefs.HasKey("masterVolume")) PlayerPrefs.SetFloat("masterVolume", 0.5f);
        if (!PlayerPrefs.HasKey("musicVolume")) PlayerPrefs.SetFloat("musicVolume", 0.5f);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("sfxVolume", 0.5f);

        LoadVolume();
    }

    // Update is called once per frame
    void Update()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();

    }
    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume" , volume);
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);

    }
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);

    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();
        
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
        
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSFXVolume();
    }
}
