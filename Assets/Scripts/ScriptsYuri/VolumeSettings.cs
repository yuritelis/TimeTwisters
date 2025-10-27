using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    // Singleton instance
    public static VolumeSettings Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeVolumeSettings();
        }
    }

    void InitializeVolumeSettings()
    {
        ApplySavedVolumes();

        SetupSliders();
    }

    void SetupSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.value = PlayerPrefs.GetFloat("musicVol", 0.75f);
            musicSlider.onValueChanged.AddListener(delegate { VolMusica(); });
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", 0.75f);
            sfxSlider.onValueChanged.AddListener(delegate { VolSfx(); });
        }
    }

    public void VolMusica()
    {
        float volume = musicSlider != null ? musicSlider.value : PlayerPrefs.GetFloat("musicVol", 0.75f);
        audioMixer.SetFloat("music", VolumeToDecibels(volume));
        PlayerPrefs.SetFloat("musicVol", volume);
        PlayerPrefs.Save();
    }

    public void VolSfx()
    {
        float volume = sfxSlider != null ? sfxSlider.value : PlayerPrefs.GetFloat("sfxVol", 0.75f);
        audioMixer.SetFloat("sfx", VolumeToDecibels(volume));
        PlayerPrefs.SetFloat("sfxVol", volume);
        PlayerPrefs.Save();
    }

    private float VolumeToDecibels(float volume)
    {
        if (volume <= 0.0001f) return -80f;
        return Mathf.Log10(volume) * 20f;
    }

    void ApplySavedVolumes()
    {
        float musicVolume = PlayerPrefs.GetFloat("musicVol", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxVol", 0.75f);

        audioMixer.SetFloat("music", VolumeToDecibels(musicVolume));
        audioMixer.SetFloat("sfx", VolumeToDecibels(sfxVolume));
    }

    public void UpdateSliderReferences(Slider newMusicSlider, Slider newSfxSlider)
    {
        musicSlider = newMusicSlider;
        sfxSlider = newSfxSlider;
        SetupSliders();
    }

    public void ApplyVolumeWithoutSliders()
    {
        ApplySavedVolumes();
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("musicVol", 0.75f);
    }

    public float GetSfxVolume()
    {
        return PlayerPrefs.GetFloat("sfxVol", 0.75f);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}