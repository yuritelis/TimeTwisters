using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroLetter : MonoBehaviour
{
    [Header("Referências")]
    public GameObject player;
    public CanvasGroup cartaUI;
    public Image background;
    public KeyCode closeKey = KeyCode.E;

    [Header("Configuração")]
    public float fadeSpeed = 2f;
    public float startDelay = 0.7f;
    public float minDisplayTime = 5f;
    public bool showOnlyOnce = true;
    public string playerPrefsKey = "IntroLetter_Shown";

    private bool cartaAtiva = false;
    private bool podeFechar = false;
    private float tempoInicio;

    void Start()
    {
        Debug.Log("=== INTRO LETTER INICIADA ===");

        if (showOnlyOnce && PlayerPrefs.GetInt(playerPrefsKey, 0) == 1)
        {
            Debug.Log("Carta já foi mostrada antes, pulando...");
            SkipLetter();
            return;
        }

        InicializarCarta();
    }

    void InicializarCarta()
    {
        Debug.Log("Inicializando carta...");

        if (background != null)
        {
            background.gameObject.SetActive(true);
            background.color = Color.black;
            background.canvasRenderer.SetAlpha(1f);
            background.rectTransform.SetAsFirstSibling();
        }

        if (cartaUI != null)
        {
            cartaUI.gameObject.SetActive(true);
            cartaUI.alpha = 0f;
            cartaUI.transform.SetAsLastSibling();
        }

        if (Camera.main != null)
        {
            Camera.main.cullingMask = 0;
            Debug.Log("Câmera principal desativada - cullingMask = 0");
        }

        Time.timeScale = 0f;
        TravarJogador(true);
        cartaAtiva = true;
        tempoInicio = Time.realtimeSinceStartup;

        StartCoroutine(SequenciaCarta());
    }

    private IEnumerator SequenciaCarta()
    {
        Debug.Log("Iniciando sequência da carta...");

        yield return new WaitForSecondsRealtime(startDelay);
        Debug.Log("Delay inicial completado");

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            if (cartaUI != null) cartaUI.alpha = t;
            yield return null;
        }

        if (cartaUI != null) cartaUI.alpha = 1f;
        Debug.Log("Fade in da carta completado");

        if (Camera.main != null)
        {
            Camera.main.cullingMask = -1;
            Debug.Log("Câmera principal reativada, mas fundo preto ainda está visível");
        }


        StartCoroutine(HabilitarFechamento());
    }

    private IEnumerator HabilitarFechamento()
    {
        Debug.Log($"Aguardando {minDisplayTime} segundos para permitir fechamento...");
        yield return new WaitForSecondsRealtime(minDisplayTime);

        podeFechar = true;
        Debug.Log("AGORA PODE FECHAR A CARTA! Use E ou Clique do Mouse");
    }

    void Update()
    {
        if (!cartaAtiva) return;

        bool inputE = Input.GetKeyDown(closeKey);
        bool inputMouse = Input.GetMouseButtonDown(0);

        if (inputE || inputMouse)
        {
            Debug.Log($"Input detectado - E: {inputE}, Mouse: {inputMouse}, PodeFechar: {podeFechar}");

            if (podeFechar)
            {
                Debug.Log("Iniciando fechamento da carta!");
                StartCoroutine(FecharCarta());
            }
            else
            {
                float tempoDecorrido = Time.realtimeSinceStartup - tempoInicio;
                Debug.Log($"Ainda não pode fechar! Tempo decorrido: {tempoDecorrido:F1}s de {minDisplayTime}s");
            }
        }
    }

    private IEnumerator FecharCarta()
    {
        Debug.Log("Iniciando fade out da carta E do fundo preto...");

        float t = 1f;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeSpeed;

            // Fade out da carta
            if (cartaUI != null) cartaUI.alpha = t;

            // Fade out do fundo preto
            if (background != null) background.canvasRenderer.SetAlpha(t);

            yield return null;
        }

        // Limpa elementos
        if (cartaUI != null) cartaUI.gameObject.SetActive(false);
        if (background != null) background.gameObject.SetActive(false);

        if (showOnlyOnce) PlayerPrefs.SetInt(playerPrefsKey, 1);

        Debug.Log("Finalizando carta e liberando jogo...");

        Time.timeScale = 1f;
        LiberarJogador();
        cartaAtiva = false;

        Debug.Log("Carta e fundo preto finalizados com sucesso!");
    }

    void SkipLetter()
    {
        Debug.Log("Pulando carta...");

        if (Camera.main != null)
            Camera.main.cullingMask = -1;

        if (cartaUI != null) cartaUI.gameObject.SetActive(false);
        if (background != null) background.gameObject.SetActive(false);

        Time.timeScale = 1f;
        LiberarJogador();
        enabled = false;
    }

    private void TravarJogador(bool estado)
    {
        if (player == null)
        {
            Debug.LogError("Player não atribuído!");
            return;
        }

        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.canMove = !estado;
            Debug.Log($"PlayerController - canMove: {!estado}");
        }

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null)
        {
            combat.enabled = !estado;
            Debug.Log($"Player_Combat - enabled: {!estado}");
        }
    }

    private void LiberarJogador()
    {
        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.canMove = true;
            Debug.Log("PlayerController liberado - canMove: true");
        }

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null)
        {
            combat.enabled = true;
            Debug.Log("Player_Combat liberado - enabled: true");
        }
    }
}