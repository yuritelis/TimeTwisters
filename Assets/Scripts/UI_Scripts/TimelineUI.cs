using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TimelineUI : MonoBehaviour
{
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

    void Start()
    {
        panel.SetActive(false);
        Presente.gameObject.SetActive(false);
        Passado.gameObject.SetActive(false);
        Futuro.gameObject.SetActive(false);

        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        Presente.onClick.AddListener(() => ChooseTimeline(Timeline.Presente));
        Passado.onClick.AddListener(() => ChooseTimeline(Timeline.Passado));
        Futuro.onClick.AddListener(() => ChooseTimeline(Timeline.Futuro));

        DetectarTimelineAtual();
    }

    private void DetectarTimelineAtual()
    {
        string cena = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToLower();

        if (cena.Contains("passado"))
            currentTimeline = Timeline.Passado;
        else if (cena.Contains("futuro"))
            currentTimeline = Timeline.Futuro;
        else
            currentTimeline = Timeline.Presente;
    }

    public void Open(TimeTravelTilemap timeObject)
    {
        DetectarTimelineAtual();

        panel.SetActive(true);
        sanidadeBar.SetActive(false);
        Presente.gameObject.SetActive(true);
        Passado.gameObject.SetActive(true);
        Futuro.gameObject.SetActive(true);

        AtualizarEstadoDosBotoes();

        Time.timeScale = 0f;
        isPaused = true;

        if (playerInput != null)
            playerInput.enabled = false;
    }

    public void Close()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FecharUI();
        }
    }

    private void FecharUI()
    {
        panel.SetActive(false);
        sanidadeBar.SetActive(true);
        Presente.gameObject.SetActive(false);
        Passado.gameObject.SetActive(false);
        Futuro.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (playerInput != null)
            playerInput.enabled = true;
    }

    private void ChooseTimeline(Timeline timeline)
    {
        if (timeline == currentTimeline)
            return;

        FecharUI();

        if (TimeTravelSceneManager.instance != null)
        {
            TimeTravelSceneManager.instance.CarregarCena(timeline);
        }
        else
        {
            Debug.LogWarning("Nenhum TimeTravelSceneManager encontrado na cena!");
        }
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
        btn.interactable = true;
        btn.GetComponent<Image>().color = normalColor;
    }

    private void DesativarBotao(Button btn)
    {
        btn.interactable = false;
        btn.GetComponent<Image>().color = disabledColor;
    }
}
