using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimelineUI : MonoBehaviour
{
    public static TimelineUI instance;

    public GameObject panel;
    public GameObject sanidadeBar;
    public Button Presente;
    public Button Passado;
    public Button Futuro;

    public PlayerController playerController;
    public PlayerInput playerInput;

    public Color normalColor = Color.white;
    public Color disabledColor = Color.gray;

    public static bool isPaused = false;
    private Timeline currentTimeline = Timeline.Presente;

    private static bool bootstrapped = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

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
        ForceCloseOnSceneLoad();
        RecarregarReferenciasUI();
        DetectarTimelineAtual();
    }

    public void ForceCloseOnSceneLoad()
    {
        if (panel != null)
        {
            var cg = panel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }

        if (sanidadeBar != null) sanidadeBar.SetActive(true);

        if (Presente != null) Presente.gameObject.SetActive(false);
        if (Passado != null) Passado.gameObject.SetActive(false);
        if (Futuro != null) Futuro.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (playerInput != null)
            playerInput.enabled = true;
    }

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

        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (Presente != null)
        {
            Presente.onClick.RemoveAllListeners();
            Presente.onClick.AddListener(() => ChooseTimeline(Timeline.Presente));
        }

        if (Passado != null)
        {
            Passado.onClick.RemoveAllListeners();
            Passado.onClick.AddListener(() => ChooseTimeline(Timeline.Passado));
        }

        if (Futuro != null)
        {
            Futuro.onClick.RemoveAllListeners();
            Futuro.onClick.AddListener(() => ChooseTimeline(Timeline.Futuro));
        }
    }

    void Start()
    {
        DetectarTimelineAtual();

        if (!bootstrapped)
        {
            ForceCloseOnSceneLoad();
            bootstrapped = true;
        }
    }

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

    public void Open(TimeTravelTilemap _)
    {
        if (panel == null)
            return;

        var cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

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

    public void Close()
    {
        if (panel != null)
        {
            var cg = panel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }

        if (sanidadeBar != null) sanidadeBar.SetActive(true);

        if (Presente != null) Presente.gameObject.SetActive(false);
        if (Passado != null) Passado.gameObject.SetActive(false);
        if (Futuro != null) Futuro.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (playerInput != null)
            playerInput.enabled = true;
    }

    private void ChooseTimeline(Timeline timeline)
    {
        if (timeline == currentTimeline)
            return;

        Close();

        if (TimeTravelSceneManager.instance != null)
            TimeTravelSceneManager.instance.CarregarCena(timeline);
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
