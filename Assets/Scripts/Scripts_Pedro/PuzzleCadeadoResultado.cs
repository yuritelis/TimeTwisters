using UnityEngine;
using System.Collections;
using TMPro;

public class PuzzleCadeadoResultado : MonoBehaviour
{
    [Header("Referências")]
    public PuzzleCadeado puzzle;
    public Dialogo dialogoDepoisDaImagem;
    public GameObject imagemFinal;
    public CanvasGroup imagemCanvas;
    public float fadeSpeed = 2f;
    public float minDisplayTime = 2f;
    public KeyCode fecharKey = KeyCode.E;

    private bool imagemAtiva = false;
    private bool podeFechar = false;
    private bool puzzleResolvido = false;
    private GameObject player;

    private void Start()
    {
        imagemFinal.SetActive(false);
        imagemCanvas.alpha = 0f;
    }

    private void Update()
    {
        if (!puzzleResolvido && puzzle.respostaCorreta)
        {
            puzzleResolvido = true;
            StartCoroutine(SequenciaImagem());
        }

        if (imagemAtiva && podeFechar && Input.GetKeyDown(fecharKey))
        {
            StartCoroutine(FecharImagem());
        }
    }

    private IEnumerator SequenciaImagem()
    {
        TravarJogador(true);

        imagemFinal.SetActive(true);
        imagemCanvas.alpha = 0f;
        imagemAtiva = true;
        podeFechar = false;

        float t = 0;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            imagemCanvas.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(minDisplayTime);
        podeFechar = true;
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

        imagemFinal.SetActive(false);
        imagemAtiva = false;

        TravarJogador(false);

        yield return null;
        yield return null;

        if (DialogoManager.Instance != null)
        {
            DialogoManager.Instance.StartDialogo(dialogoDepoisDaImagem);
        }

        yield return StartCoroutine(EsperarDialogoEAvancar());
    }

    private IEnumerator EsperarDialogoEAvancar()
    {
        while (DialogoManager.Instance.dialogoAtivoPublico)
            yield return null;

        StoryProgressManager.instance.AvancarEtapa();
    }

    private void TravarJogador(bool estado)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        if (controller != null) controller.canMove = !estado;

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null) combat.enabled = !estado;
    }
}
