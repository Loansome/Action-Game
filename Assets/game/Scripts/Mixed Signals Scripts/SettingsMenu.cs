using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider saveSlider;

    private void Awake()
    {
        saveSlider.value = PlayerPrefs.GetFloat("sliderSavedNumber");
    }
    private void Update()
    {
        PlayerPrefs.SetFloat("sliderSavedNumber", saveSlider.value);
    }

    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }
}
