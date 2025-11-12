using UnityEngine;
using System.Collections;

public class EstanteInterativa : MonoBehaviour, IInteractable
{
    [Header("Referências")]
    public Dialogo dialogo;
    public GameObject imagemFinal;
    public CanvasGroup imagemCanvas;
    public KeyCode closeKey = KeyCode.E;
    public float fadeSpeed = 2f;
    public float minDisplayTime = 3f;

    private bool jaInteragiu = false;
    private bool imagemAtiva = false;
    private bool podeFechar = false;
    private float tempoInicio;
    private GameObject player;

    public bool CanInteract()
    {
        return !jaInteragiu && !imagemAtiva;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        jaInteragiu = true;
        player = GameObject.FindGameObjectWithTag("Player");

        TravarJogador(true);

        if (DialogoManager.Instance == null)
        {
            Debug.LogWarning("⚠️ Nenhum DialogoManager encontrado — exibindo imagem direta.");
            StartCoroutine(MostrarImagem());
            return;
        }

        DialogoManager.Instance.StartDialogo(dialogo);
        StartCoroutine(EsperarDialogo());
    }

    private IEnumerator EsperarDialogo()
    {
        float timeout = 10f;
        float elapsed = 0f;

        while (DialogoManager.Instance != null && DialogoManager.Instance.dialogoAtivoPublico && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        TravarJogador(true);

        Debug.Log("🧩 Detecção: diálogo terminou, iniciando exibição da imagem...");
        yield return new WaitForSecondsRealtime(0.3f);
        yield return MostrarImagem();
    }


    private IEnumerator MostrarImagem()
    {
        if (imagemFinal == null || imagemCanvas == null)
        {
            Debug.LogWarning("⚠️ Imagem final ou CanvasGroup não atribuídos na EstanteInterativa!");
            yield break;
        }

        imagemFinal.SetActive(true);
        imagemCanvas.alpha = 0f;
        imagemAtiva = true;
        podeFechar = false;

        TravarJogador(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            imagemCanvas.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        imagemCanvas.alpha = 1f;
        tempoInicio = Time.realtimeSinceStartup;
        StartCoroutine(HabilitarFechamento());
        Debug.Log("📖 Imagem exibida após o diálogo.");
    }

    private IEnumerator HabilitarFechamento()
    {
        yield return new WaitForSecondsRealtime(minDisplayTime);
        podeFechar = true;
        Debug.Log("✅ Agora é possível fechar a imagem (pressione E).");
    }

    private void Update()
    {
        if (imagemAtiva && podeFechar && Input.GetKeyDown(closeKey))
        {
            StartCoroutine(FecharImagem());
        }
    }

    private IEnumerator FecharImagem()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeSpeed;
            imagemCanvas.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        imagemCanvas.alpha = 0f;
        imagemFinal.SetActive(false);
        imagemAtiva = false;

        TravarJogador(false);
        Debug.Log("📕 Imagem fechada com sucesso.");
    }

    private void TravarJogador(bool estado)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.canMove = !estado;

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null)
            combat.enabled = !estado;

        Debug.Log(estado ? "🧊 Player travado (jornal/diálogo)." : "🔥 Player liberado.");
    }
}
