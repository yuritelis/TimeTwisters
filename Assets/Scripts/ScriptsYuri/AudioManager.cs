using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("> Audio Source <")]
    [SerializeField] AudioSource fonteMus;
    [SerializeField] AudioSource fonteSfx;

    [Header("> Audio Clips (Música) <")]
    public AudioClip menuBgm;
    public AudioClip pasBgm;
    public AudioClip presBgm;
    public AudioClip futBgm;
    public AudioClip bossBgm;

    [Header("> Audio Clips (SFX) <")]
    public AudioClip botClick;

    private string currentScene;
    private string currentTimeline;
    private bool inBossZone = false;
    private TimeTravelTilemap timeTravel;

    public static AudioManager instance;

    void Awake()
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
        {
            fonteMus.loop = true;
        }
    }

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;

        FindTimeTravelReference();
        UpdateMusic();

        VolumeSettings.Instance.ApplyVolumeWithoutSliders();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        inBossZone = false;
        currentTimeline = "";

        FindTimeTravelReference();
        UpdateMusic();
    }

    private void FindTimeTravelReference()
    {
        timeTravel = FindFirstObjectByType<TimeTravelTilemap>();
    }

    private void Update()
    {
        if (timeTravel != null)
        {
            string newTimeline = "";

            switch (timeTravel.CurrentTimeline)
            {
                case Timeline.Presente:
                    newTimeline = "Presente";
                    break;
                case Timeline.Passado:
                    newTimeline = "Passado";
                    break;
                case Timeline.Futuro:
                    newTimeline = "Futuro";
                    break;
                default:
                    newTimeline = "Presente";
                    break;
            }

            if (currentTimeline != newTimeline)
            {
                currentTimeline = newTimeline;
                UpdateMusic();
            }
        }
    }

    public void SetCurrentTimeline(string timeline)
    {
        currentTimeline = timeline;
        UpdateMusic();
    }

    public void SetBossZone(bool inZone)
    {
        inBossZone = inZone;
        UpdateMusic();
    }

    public void OnBossDefeated()
    {
        inBossZone = false;
        UpdateMusic();
    }

    void UpdateMusic()
    {
        AudioClip clipToPlay = null;

        if (inBossZone && bossBgm != null)
        {
            clipToPlay = bossBgm;
        }
        else if (currentScene == "TitleScreen" && menuBgm != null)
        {
            clipToPlay = menuBgm;
        }
        else if (currentScene == "Alpha")
        {
            switch (currentTimeline)
            {
                case "Passado":
                    if (pasBgm != null)
                    {
                        clipToPlay = pasBgm;
                    }
                    break;
                case "Presente":
                    if (presBgm != null)
                    {
                        clipToPlay = presBgm;
                    }
                    break;
                case "Futuro":
                    if (futBgm != null)
                    {
                        clipToPlay = futBgm;
                    }
                    break;
                default:
                    if (presBgm != null)
                    {
                        clipToPlay = presBgm;
                    }
                    break;
            }
        }

        if (clipToPlay != null)
        {
            PlayMus(clipToPlay);
        }
        else
        {
            if (fonteMus.isPlaying)
            {
                fonteMus.Stop();
                fonteMus.clip = null;
            }
        }
    }

    void PlayMus(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (fonteMus == null)
        {
            return;
        }

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

    public void StopMusic()
    {
        if (fonteMus.isPlaying)
        {
            fonteMus.Stop();
            fonteMus.clip = null;
        }
    }

    public void SetMusicPaused(bool paused)
    {
        if (paused)
        {
            fonteMus.Pause();
        }
        else
        {
            fonteMus.UnPause();
        }
    }

    public void SetMusicVolume(float volume)
    {
        fonteMus.volume = Mathf.Clamp01(volume);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (fonteSfx != null && clip != null)
        {
            fonteSfx.PlayOneShot(clip);
        }
    }
}