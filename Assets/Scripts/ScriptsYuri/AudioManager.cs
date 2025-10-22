using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("> Audio Source <")]
    [SerializeField] AudioSource fonteMus;
    [SerializeField] AudioSource fonteSfx;

    [Header("> Audio Clips (M�sica) <")]
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

    // Singleton para f�cil acesso
    public static AudioManager instance;

    void Awake()
    {
        // Implementa��o do Singleton
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

        // Garantir que fonteMus est� configurado corretamente
        if (fonteMus != null)
        {
            fonteMus.loop = true;
        }
        else
        {
            Debug.LogError("fonteMus n�o est� atribu�do no Inspector!");
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

        // Buscar nova refer�ncia do TimeTravel na nova cena
        FindTimeTravelReference();
        UpdateMusic();
    }

    // Buscar refer�ncia do TimeTravelTilemap na cena
    private void FindTimeTravelReference()
    {
        timeTravel = FindObjectOfType<TimeTravelTilemap>();
        if (timeTravel != null)
        {
            Debug.Log("TimeTravelTilemap encontrado na cena!");
        }
        else
        {
            Debug.LogWarning("TimeTravelTilemap n�o encontrado na cena.");
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
                    newTimeline = "Presente"; // Valor padr�o
                    break;
            }

            // S� atualizar se a timeline mudou
            if (currentTimeline != newTimeline)
            {
                currentTimeline = newTimeline;
                Debug.Log($"Timeline mudou para: {currentTimeline}");
                UpdateMusic();
            }
        }
    }

    // M�todo manual para for�ar mudan�a (opcional)
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
            Debug.Log("Tocando m�sica do Boss");
        }
        else if (currentScene == "TitleScreen" && menuBgm != null)
        {
            clipToPlay = menuBgm;
            Debug.Log("Tocando m�sica do Menu");
        }
        else if (currentScene == "teste_yuri")
        {
            switch (currentTimeline)
            {
                case "Passado":
                    if (pasBgm != null)
                    {
                        clipToPlay = pasBgm;
                        Debug.Log("Tocando m�sica do Passado");
                    }
                    break;
                case "Presente":
                    if (presBgm != null)
                    {
                        clipToPlay = presBgm;
                        Debug.Log("Tocando m�sica do Presente");
                    }
                    break;
                case "Futuro":
                    if (futBgm != null)
                    {
                        clipToPlay = futBgm;
                        Debug.Log("Tocando m�sica do Futuro");
                    }
                    break;
                default:
                    if (presBgm != null)
                    {
                        clipToPlay = presBgm; // M�sica padr�o
                        Debug.Log("Tocando m�sica padr�o (Presente)");
                    }
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
            if (fonteMus.isPlaying)
            {
                fonteMus.Stop();
                fonteMus.clip = null;
                Debug.Log("Parando m�sica - nenhum clip v�lido");
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

        if (fonteMus == null)
        {
            Debug.LogError("fonteMus � null! Verifique o Inspector.");
            return;
        }

        // S� troca de m�sica se for uma m�sica diferente
        if (fonteMus.clip != clip)
        {
            fonteMus.Stop();
            fonteMus.clip = clip;
            fonteMus.Play();
            Debug.Log($"Tocando m�sica: {clip.name}");
        }
        // Se for a mesma m�sica mas n�o est� tocando, toca
        else if (!fonteMus.isPlaying)
        {
            fonteMus.Play();
            Debug.Log($"Retomando m�sica: {clip.name}");
        }
    }

    // M�todo para parar a m�sica completamente
    public void StopMusic()
    {
        if (fonteMus.isPlaying)
        {
            fonteMus.Stop();
            fonteMus.clip = null;
        }
    }

    // M�todo para pausar/despausar m�sica
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

    // Importante: Remover o event listener quando o objeto for destru�do
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

    // DEBUG: M�todo para verificar o estado atual
    public void DebugAudioState()
    {
        Debug.Log($"Cena: {currentScene}, Timeline: {currentTimeline}, BossZone: {inBossZone}");
        Debug.Log($"M�sica atual: {(fonteMus.clip != null ? fonteMus.clip.name : "Nenhuma")}");
        Debug.Log($"Tocando: {fonteMus.isPlaying}");
        Debug.Log($"TimeTravel referencia: {(timeTravel != null ? "Encontrado" : "Nulo")}");

        if (timeTravel != null)
        {
            Debug.Log($"Timeline atual do TimeTravel: {timeTravel.CurrentTimeline}");
        }
    }
}