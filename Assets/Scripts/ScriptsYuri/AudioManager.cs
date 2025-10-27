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
        if (timeTravel != null)
        {
            Debug.Log("TimeTravelTilemap encontrado na cena!");
        }
        else
        {
            Debug.LogWarning("TimeTravelTilemap não encontrado na cena.");
        }
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
                Debug.Log($"Timeline mudou para: {currentTimeline}");
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
            Debug.Log("Tocando música do Boss");
        }
        else if (currentScene == "TitleScreen" && menuBgm != null)
        {
            clipToPlay = menuBgm;
            Debug.Log("Tocando música do Menu");
        }
        else if (currentScene == "Alpha")
        {
            switch (currentTimeline)
            {
                case "Passado":
                    if (pasBgm != null)
                    {
                        clipToPlay = pasBgm;
                        Debug.Log("Tocando música do Passado");
                    }
                    break;
                case "Presente":
                    if (presBgm != null)
                    {
                        clipToPlay = presBgm;
                        Debug.Log("Tocando música do Presente");
                    }
                    break;
                case "Futuro":
                    if (futBgm != null)
                    {
                        clipToPlay = futBgm;
                        Debug.Log("Tocando música do Futuro");
                    }
                    break;
                default:
                    if (presBgm != null)
                    {
                        clipToPlay = presBgm;
                        Debug.Log("Tocando música padrão (Presente)");
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
                Debug.Log("Parando música - nenhum clip válido");
            }
        }
    }

    void PlayMus(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip é null!");
            return;
        }

        if (fonteMus == null)
        {
            Debug.LogError("fonteMus é null! Verifique o Inspector.");
            return;
        }

        if (fonteMus.clip != clip)
        {
            fonteMus.Stop();
            fonteMus.clip = clip;
            fonteMus.Play();
            Debug.Log($"Tocando música: {clip.name}");
        }
        else if (!fonteMus.isPlaying)
        {
            fonteMus.Play();
            Debug.Log($"Retomando música: {clip.name}");
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

    public void DebugAudioState()
    {
        Debug.Log($"Cena: {currentScene}, Timeline: {currentTimeline}, BossZone: {inBossZone}");
        Debug.Log($"Música atual: {(fonteMus.clip != null ? fonteMus.clip.name : "Nenhuma")}");
        Debug.Log($"Tocando: {fonteMus.isPlaying}");
        Debug.Log($"TimeTravel referencia: {(timeTravel != null ? "Encontrado" : "Nulo")}");

        if (timeTravel != null)
        {
            Debug.Log($"Timeline atual do TimeTravel: {timeTravel.CurrentTimeline}");
        }
    }
}