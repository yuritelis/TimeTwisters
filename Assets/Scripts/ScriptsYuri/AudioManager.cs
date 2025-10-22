using UnityEngine.SceneManagement;
using UnityEngine;

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

    // Singleton para fácil acesso
    public static AudioManager instance;

    void Awake()
    {
        // Implementação do Singleton
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

        // Garantir que fonteMus está configurado corretamente
        if (fonteMus != null)
        {
            fonteMus.loop = true;
        }
        else
        {
            Debug.LogError("fonteMus não está atribuído no Inspector!");
        }
    }

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Buscar o TimeTravelTilemap na cena
        FindTimeTravelReference();
        UpdateMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        inBossZone = false; // Reset boss zone ao mudar de cena
        currentTimeline = ""; // Reset timeline

        // Buscar nova referência do TimeTravel na nova cena
        FindTimeTravelReference();
        UpdateMusic();
    }

    // Buscar referência do TimeTravelTilemap na cena
    private void FindTimeTravelReference()
    {
        timeTravel = FindObjectOfType<TimeTravelTilemap>();
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
        // Verificar automaticamente a timeline atual
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
                    newTimeline = "Presente"; // Valor padrão
                    break;
            }

            // Só atualizar se a timeline mudou
            if (currentTimeline != newTimeline)
            {
                currentTimeline = newTimeline;
                Debug.Log($"Timeline mudou para: {currentTimeline}");
                UpdateMusic();
            }
        }
    }

    // Método manual para forçar mudança (opcional)
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
            Debug.Log("Tocando música do Boss");
        }
        else if (currentScene == "TitleScreen" && menuBgm != null)
        {
            clipToPlay = menuBgm;
            Debug.Log("Tocando música do Menu");
        }
        else if (currentScene == "teste_yuri")
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
                        clipToPlay = presBgm; // Música padrão
                        Debug.Log("Tocando música padrão (Presente)");
                    }
                    break;
            }
        }

        // Só toca se o clip for válido
        if (clipToPlay != null)
        {
            PlayMus(clipToPlay);
        }
        else
        {
            // Se não há música para tocar, para a música atual
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

        // Só troca de música se for uma música diferente
        if (fonteMus.clip != clip)
        {
            fonteMus.Stop();
            fonteMus.clip = clip;
            fonteMus.Play();
            Debug.Log($"Tocando música: {clip.name}");
        }
        // Se for a mesma música mas não está tocando, toca
        else if (!fonteMus.isPlaying)
        {
            fonteMus.Play();
            Debug.Log($"Retomando música: {clip.name}");
        }
    }

    // Método para parar a música completamente
    public void StopMusic()
    {
        if (fonteMus.isPlaying)
        {
            fonteMus.Stop();
            fonteMus.clip = null;
        }
    }

    // Método para pausar/despausar música
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

    // Configurar volume
    public void SetMusicVolume(float volume)
    {
        fonteMus.volume = Mathf.Clamp01(volume);
    }

    // Importante: Remover o event listener quando o objeto for destruído
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

    // DEBUG: Método para verificar o estado atual
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