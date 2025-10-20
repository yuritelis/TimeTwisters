using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("> Audio Source <")]
    [SerializeField] AudioSource fonteMus;
    [SerializeField] AudioSource fonteSfx;

    [Header("Background Musics")]
    public AudioClip menuBgm;
    public AudioClip pasBgm;
    public AudioClip presBgm;
    public AudioClip futBgm;
    public AudioClip bossBgm;

    private string currentScene;
    private string currentTimeline;
    private bool inBossZone = false;
    private AudioSource musicSource;

    // Singleton para f�cil acesso
    public static AudioManager instance;

    void Awake()
    {
        // Implementa��o do Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializar AudioSource ANTES do Start
            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true; // Importante para m�sica de fundo
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        inBossZone = false; // Reset boss zone ao mudar de cena
        currentTimeline = ""; // Reset timeline
        UpdateMusic();
    }

    // Chamado quando muda de timeline
    public void SetCurrentTimeline(string timeline)
    {
        currentTimeline = timeline;
        UpdateMusic();
    }

    // Chamado quando entra/sai da zona do boss
    public void SetBossZone(bool inZone)
    {
        inBossZone = inZone;
        UpdateMusic();
    }

    // Chamado quando o boss morre
    public void OnBossDefeated()
    {
        inBossZone = false;
        UpdateMusic();
    }

    void UpdateMusic()
    {
        AudioClip clipToPlay = null;

        // Prioridade: Boss > Timeline > Cena
        if (inBossZone && bossBgm != null)
        {
            clipToPlay = bossBgm;
        }
        else if (currentScene == "MainMenu" && menuBgm != null)
        {
            clipToPlay = menuBgm;
        }
        else if (currentScene == "teste_yuri")
        {
            switch (currentTimeline)
            {
                case "Past":
                    clipToPlay = pasBgm;
                    break;
                case "Present":
                    clipToPlay = presBgm;
                    break;
                case "Future":
                    clipToPlay = futBgm;
                    break;
                default:
                    clipToPlay = presBgm; // M�sica padr�o
                    break;
            }
        }

        // S� toca se o clip for v�lido
        if (clipToPlay != null)
        {
            PlayMus(clipToPlay);
        }
        else
        {
            // Se n�o h� m�sica para tocar, para a m�sica atual
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
                musicSource.clip = null;
            }
        }
    }

    void PlayMus(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip � null!");
            return;
        }

        // S� troca de m�sica se for uma m�sica diferente
        if (musicSource.clip != clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
        }
        // Se for a mesma m�sica mas n�o est� tocando, toca
        else if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // M�todo para parar a m�sica completamente
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }

    // M�todo para pausar/despausar m�sica
    public void SetMusicPaused(bool paused)
    {
        if (paused)
        {
            musicSource.Pause();
        }
        else
        {
            musicSource.UnPause();
        }
    }

    // Configurar volume
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    // Importante: Remover o event listener quando o objeto for destru�do
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlaySFX(AudioClip clip)
    {
        fonteSfx.PlayOneShot(clip);
    }
}