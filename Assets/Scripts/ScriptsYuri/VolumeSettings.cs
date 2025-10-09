using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("musicVol"))
        {
            LoadVolume();
        }
        else
        {
            VolMusica();
            VolSfx();
        }
    }

    void Update()
    {
        
    }

    public void VolMusica()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVol", volume);
    }
    
    public void VolSfx()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVol", volume);
    }

    void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVol");
        musicSlider.value = PlayerPrefs.GetFloat("sfxVol");

        VolMusica();
        VolSfx();
    }
}
