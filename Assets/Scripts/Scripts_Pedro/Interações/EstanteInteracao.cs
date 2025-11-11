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

    public bool CanInteract()
    {
        return !jaInteragiu && !imagemAtiva;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        jaInteragiu = true;

        DialogoManager.Instance.StartDialogo(dialogo);

        DialogoManager.Instance.StartCoroutine(EsperarDialogo());
    }

    private IEnumerator EsperarDialogo()
    {
        yield return new WaitUntil(() => !DialogoAtivo());

        yield return MostrarImagem();
    }

    private bool DialogoAtivo()
    {
        if (DialogoManager.Instance == null) return false;

        return DialogoManager.Instance.dialogoAtivoPublico;
    }


    private IEnumerator MostrarImagem()
    {
        if (imagemFinal == null || imagemCanvas == null)
        {
            Debug.LogWarning("⚠️ Imagem final não atribuída na estante!");
            yield break;
        }

        imagemFinal.SetActive(true);
        imagemCanvas.alpha = 0f;
        imagemAtiva = true;
        podeFechar = false;

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

        Debug.Log("📕 Imagem fechada com sucesso.");
    }
}
