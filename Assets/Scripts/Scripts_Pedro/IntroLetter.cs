using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IntroLetter : MonoBehaviour
{
    [Header("Referências")]
    public GameObject player;
    public CanvasGroup cartaUI;
    public Image background;
    public KeyCode closeKey = KeyCode.E;
    public CameraSegue cameraSegue;                // 🎥 adiciona referência à câmera

    [Header("Configuração")]
    public float fadeSpeed = 2f;
    public float startDelay = 0.7f;
    public float minDisplayTime = 5f;
    public bool showOnlyOnce = true;
    public string playerPrefsKey = "IntroLetter_Shown";

    [Header("Cutscene Pós-Carta")]
    [Tooltip("Pontos de câmera a serem visitados após a carta fechar.")]
    public List<Transform> cameraPoints = new List<Transform>();
    public float cameraMoveSpeed = 2f;
    public float cameraHoldTime = 2f;

    [Header("Diálogo Pós-Carta")]
    [Tooltip("Diálogo exibido automaticamente após a cutscene da carta.")]
    public Dialogo dialogoPosCarta;
    [TextArea(2, 4)]
    public string falaPadrao = "Preciso descobrir o que está acontecendo neste lugar...";

    private bool cartaAtiva = false;
    private bool podeFechar = false;
    private float tempoInicio;

    void Start()
    {
        if (showOnlyOnce && PlayerPrefs.GetInt(playerPrefsKey, 0) == 1)
        {
            SkipLetter();
            return;
        }

        if (cameraSegue == null)
            cameraSegue = FindFirstObjectByType<CameraSegue>();

        InicializarCarta();
    }

    void InicializarCarta()
    {
        if (player != null)
        {
            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetFloat("InputX", 0f);
                anim.SetFloat("InputY", 1f);
                anim.SetFloat("LastInputX", 0f);
                anim.SetFloat("LastInputY", 1f);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
            }
        }

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
            Camera.main.cullingMask = 0;

        Time.timeScale = 0f;
        TravarJogador(true);
        cartaAtiva = true;
        tempoInicio = Time.realtimeSinceStartup;

        StartCoroutine(SequenciaCarta());
    }

    private IEnumerator SequenciaCarta()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            if (cartaUI != null) cartaUI.alpha = t;
            yield return null;
        }

        if (Camera.main != null)
            Camera.main.cullingMask = -1;

        StartCoroutine(HabilitarFechamento());
    }

    private IEnumerator HabilitarFechamento()
    {
        yield return new WaitForSecondsRealtime(minDisplayTime);
        podeFechar = true;
    }

    void Update()
    {
        if (!cartaAtiva) return;

        if ((Input.GetKeyDown(closeKey) || Input.GetMouseButtonDown(0)) && podeFechar)
        {
            StartCoroutine(FecharCarta());
        }
    }

    private IEnumerator FecharCarta()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeSpeed;
            if (cartaUI != null) cartaUI.alpha = t;
            if (background != null) background.canvasRenderer.SetAlpha(t);
            yield return null;
        }

        if (showOnlyOnce) PlayerPrefs.SetInt(playerPrefsKey, 1);
        Time.timeScale = 1f;
        cartaAtiva = false;

        StartCoroutine(DialogoPosCarta());
        yield return StartCoroutine(ExecutarCutscene());

        if (cartaUI != null) cartaUI.gameObject.SetActive(false);
        if (background != null) background.gameObject.SetActive(false);
    }


    private IEnumerator ExecutarCutscene()
    {
        if (cameraSegue == null || cameraPoints.Count == 0)
            yield break;

        TravarJogador(true);

        foreach (var point in cameraPoints)
        {
            if (point == null) continue;

            cameraSegue.BeginTemporaryFocus(point);
            yield return StartCoroutine(MoverCameraLentamente(cameraSegue.transform, point.position));
            yield return new WaitForSeconds(cameraHoldTime);
        }

        // Volta para o player
        cameraSegue.BeginTemporaryFocus(player.transform);
        yield return StartCoroutine(MoverCameraLentamente(cameraSegue.transform, player.transform.position));
        cameraSegue.EndTemporaryFocus();
    }

    private IEnumerator MoverCameraLentamente(Transform cam, Vector3 destino)
    {
        float t = 0;
        Vector3 inicio = cam.position;
        destino.z = cam.position.z;

        while (t < 1)
        {
            t += Time.deltaTime * cameraMoveSpeed;
            cam.position = Vector3.Lerp(inicio, destino, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }

    private IEnumerator DialogoPosCarta()
    {
        Dialogo dlg = dialogoPosCarta ?? CriarDialogoAutomatico(falaPadrao);

        if (DialogoManager.Instance != null)
        {
            DialogoManager.Instance.StartDialogo(dlg);
            while (DialogoManager.Instance.dialogoAtivoPublico)
                yield return null;
        }
        else
        {
            Debug.Log($"🗣️ Julie: {falaPadrao}");
            yield return new WaitForSeconds(2f);
        }

        LiberarJogador();
    }

    void SkipLetter()
    {
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
        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.canMove = !estado;

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null)
            combat.enabled = !estado;
    }

    private void LiberarJogador()
    {
        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.canMove = true;

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null)
            combat.enabled = true;
    }

    private Dialogo CriarDialogoAutomatico(string texto)
    {
        return new Dialogo
        {
            dialogoFalas = new List<DialogoFalas>
            {
                new DialogoFalas
                {
                    personagem = new PersoInfos { nome = "Julie", portrait = null },
                    fala = texto
                }
            }
        };
    }
}
