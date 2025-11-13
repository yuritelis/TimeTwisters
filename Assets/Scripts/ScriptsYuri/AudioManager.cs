using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("> Audio Source <")]
    [SerializeField] AudioSource fonteMus;
    [SerializeField] AudioSource fonteSfx;
    [SerializeField] AudioSource fonteVoz;

    [Header("> Audio Clips (Música) <")]
    public AudioClip menuBgm;
    public AudioClip deathScreenBgm;
    public AudioClip pasBgm;
    public AudioClip presBgm;
    public AudioClip futBgm;
    public AudioClip bossBgm;
    public AudioClip preBossBgm;
    public AudioClip perseguicaoBgm;
    public AudioClip vitrolaBgm;
    public AudioClip vitrolaDistortBgm;

    [Header("> Audio Clips (SFX) <")]
    public AudioClip botClick;

    public static AudioManager instance;

    private bool inBossZone = false;

    private bool musicaForcada = false;
    private AudioClip musicaOriginal = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fonteMus != null)
            fonteMus.loop = true;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateMusic();

        if (VolumeSettings.Instance != null)
            VolumeSettings.Instance.ApplyVolumeWithoutSliders();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inBossZone = false;
        musicaForcada = false;
        UpdateMusic();
    }

    private void UpdateMusic()
    {
        if (musicaForcada)
            return;

        string cena = SceneManager.GetActiveScene().name.ToLower();
        AudioClip clipToPlay = null;

        if (cena.Contains("titlescreen"))
        {
            clipToPlay = menuBgm;
        }
        else if (cena.Contains("sagu"))
        {
            clipToPlay = presBgm;
        }
        else if (inBossZone && bossBgm != null)
        {
            clipToPlay = bossBgm;
        }
        else
        {
            if (cena.Contains("passado"))
                clipToPlay = pasBgm;
            else if (cena.Contains("futuro"))
                clipToPlay = futBgm;
            else
                clipToPlay = presBgm;
        }

        PlayMusInternal(clipToPlay);
    }

    private void PlayMusInternal(AudioClip clip)
    {
        if (clip == null || fonteMus == null)
            return;

        if (fonteMus.clip != clip)
        {
            fonteMus.Stop();
            fonteMus.clip = clip;
            fonteMus.Play();
        }
        else if (!fonteMus.isPlaying)
        {
            fonteMus.Play();
        }
    }

    public void TocarMusicaCutscene(AudioClip musica)
    {
        if (musica == null || fonteMus == null) return;

        if (!musicaForcada)
            musicaOriginal = fonteMus.clip;

        musicaForcada = true;

        fonteMus.Stop();
        fonteMus.clip = musica;
        fonteMus.Play();
    }

    public void RestaurarMusicaNormal()
    {
        musicaForcada = false;
        UpdateMusic();
    }

    public void StopMusic()
    {
        if (fonteMus != null)
        {
            fonteMus.Stop();
            fonteMus.clip = null;
        }
    }

    public void SetMusicPaused(bool paused)
    {
        if (paused) fonteMus.Pause();
        else fonteMus.UnPause();
    }

    public void SetMusicVolume(float volume)
    {
        fonteMus.volume = Mathf.Clamp01(volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (fonteSfx != null && clip != null)
            fonteSfx.PlayOneShot(clip, Mathf.Clamp01(volume));
    }


    public void SetBossZone(bool inZone)
    {
        inBossZone = inZone;
        UpdateMusic();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
