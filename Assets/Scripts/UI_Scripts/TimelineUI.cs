using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimelineUI : MonoBehaviour
{
    public static TimelineUI instance;

    [Header("Referências de UI")]
    public GameObject panel;
    public GameObject sanidadeBar;
    public Button Presente;
    public Button Passado;
    public Button Futuro;

    [Header("Controle do Jogador")]
    public PlayerController playerController;
    public PlayerInput playerInput;

    [Header("Configuração de Cor")]
    public Color normalColor = Color.white;
    public Color disabledColor = Color.gray;

    public static bool isPaused = false;
    private Timeline currentTimeline = Timeline.Presente;

    private static bool bootstrapped = false;

    // ==========================================================
    // SINGLETON
    // ==========================================================
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
    }

    // ==========================================================
    // SCENE LOADED
    // ==========================================================
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceCloseOnSceneLoad();   // <<<<<< ESSENCIAL
        RecarregarReferenciasUI();
        DetectarTimelineAtual();
    }

    // Fecha tudo ANTES de buscar referências e atualizar UI
    public void ForceCloseOnSceneLoad()
    {
        if (panel != null) panel.SetActive(false);
        if (sanidadeBar != null) sanidadeBar.SetActive(true);

        if (Presente != null) Presente.gameObject.SetActive(false);
        if (Passado != null) Passado.gameObject.SetActive(false);
        if (Futuro != null) Futuro.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (playerInput != null)
            playerInput.enabled = true;
    }

    // ==========================================================
    // REFERÊNCIAS
    // ==========================================================
    private void RecarregarReferenciasUI()
    {
        if (panel == null)
            panel = GameObject.Find("TimelinePanel");

        if (sanidadeBar == null)
            sanidadeBar = GameObject.Find("SanidadeBar");

        if (Presente == null || Passado == null || Futuro == null)
        {
            Button[] botoes = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (var b in botoes)
            {
                if (b.name.Contains("Presente")) Presente = b;
                else if (b.name.Contains("Passado")) Passado = b;
                else if (b.name.Contains("Futuro")) Futuro = b;
            }
        }
    }

    // ==========================================================
    // START
    // ==========================================================
    void Start()
    {
        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (Presente != null)
            Presente.onClick.AddListener(() => ChooseTimeline(Timeline.Presente));
        if (Passado != null)
            Passado.onClick.AddListener(() => ChooseTimeline(Timeline.Passado));
        if (Futuro != null)
            Futuro.onClick.AddListener(() => ChooseTimeline(Timeline.Futuro));

        DetectarTimelineAtual();

        // Primeira inicialização
        if (!bootstrapped)
        {
            ForceCloseOnSceneLoad();
            bootstrapped = true;
        }
    }

    // ==========================================================
    // TIMELINE ATUAL
    // ==========================================================
    private void DetectarTimelineAtual()
    {
        string cena = SceneManager.GetActiveScene().name.ToLower();

        if (cena.Contains("passado"))
            currentTimeline = Timeline.Passado;
        else if (cena.Contains("futuro"))
            currentTimeline = Timeline.Futuro;
        else
            currentTimeline = Timeline.Presente;
    }

    // ==========================================================
    // ABRIR UI
    // ==========================================================
    public void Open(TimeTravelTilemap timeObject)
    {
        if (panel == null) return;

        panel.SetActive(true);
        if (sanidadeBar != null) sanidadeBar.SetActive(false);

        Presente.gameObject.SetActive(true);
        Passado.gameObject.SetActive(true);
        Futuro.gameObject.SetActive(true);

        DetectarTimelineAtual();
        AtualizarEstadoDosBotoes();

        Time.timeScale = 0f;
        isPaused = true;

        if (playerInput != null)
            playerInput.enabled = false;
    }

    // ==========================================================
    // FECHAR UI
    // ==========================================================
    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        if (sanidadeBar != null) sanidadeBar.SetActive(true);

        if (Presente != null) Presente.gameObject.SetActive(false);
        if (Passado != null) Passado.gameObject.SetActive(false);
        if (Futuro != null) Futuro.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (playerInput != null)
            playerInput.enabled = true;
    }

    // ==========================================================
    // BOTÕES
    // ==========================================================
    private void ChooseTimeline(Timeline timeline)
    {
        if (timeline == currentTimeline)
            return;

        Close();

        if (TimeTravelSceneManager.instance != null)
            TimeTravelSceneManager.instance.CarregarCena(timeline);
        else
            Debug.LogWarning("Nenhum TimeTravelSceneManager encontrado!");
    }

    private void AtualizarEstadoDosBotoes()
    {
        ResetarBotao(Presente);
        ResetarBotao(Passado);
        ResetarBotao(Futuro);

        switch (currentTimeline)
        {
            case Timeline.Presente:
                DesativarBotao(Presente);
                break;
            case Timeline.Passado:
                DesativarBotao(Passado);
                break;
            case Timeline.Futuro:
                DesativarBotao(Futuro);
                break;
        }
    }

    private void ResetarBotao(Button btn)
    {
        if (btn == null) return;
        btn.interactable = true;
        btn.GetComponent<Image>().color = normalColor;
    }

    private void DesativarBotao(Button btn)
    {
        if (btn == null) return;
        btn.interactable = false;
        btn.GetComponent<Image>().color = disabledColor;
    }
}
