using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance;

    private Dialogo dialogoData;

    [Header("Referências de UI")]
    public GameObject dialogoPanel;
    public GameObject sanidadeBar;
    public TextMeshProUGUI dialogoTxt;
    public TextMeshProUGUI personagemNome;
    public Image personagemIcon;

    [Header("Configuração")]
    public float velFala = 0.5f;

    private int dialogoIndex;
    private bool isTyping;
    private bool isDialogoAtivo;

    [HideInInspector] public bool dialogoAtivoPublico;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogo(Dialogo dialogo)
    {
        if (dialogo == null || dialogo.dialogoFalas == null || dialogo.dialogoFalas.Count == 0)
        {
            Debug.LogError("⚠️ Dados do diálogo estão vazios ou nulos!");
            return;
        }

        Debug.Log("🗨️ Iniciando diálogo...");

        isDialogoAtivo = true;
        dialogoAtivoPublico = true;

        dialogoData = dialogo;
        dialogoIndex = 0;

        dialogoPanel.SetActive(true);
        if (sanidadeBar != null) sanidadeBar.SetActive(false);

        ProxLinha();
    }

    public void ProxLinha()
    {
        if (!isDialogoAtivo || dialogoData == null) return;

        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
            dialogoTxt.text = dialogoData.dialogoFalas[dialogoIndex].fala;
            dialogoIndex++;
            return;
        }

        if (dialogoIndex >= dialogoData.dialogoFalas.Count)
        {
            FimDialogo();
            return;
        }

        var falaAtual = dialogoData.dialogoFalas[dialogoIndex];

        if (personagemNome != null)
            personagemNome.text = falaAtual.personagem.nome;

        if (personagemIcon != null && falaAtual.personagem.portrait != null)
            personagemIcon.sprite = falaAtual.personagem.portrait;

        StartCoroutine(TypeLine(falaAtual.fala));
    }

    private IEnumerator TypeLine(string fala)
    {
        isTyping = true;
        dialogoTxt.text = "";

        foreach (char letter in fala.ToCharArray())
        {
            dialogoTxt.text += letter;
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(PersoInfos.somVoz);
            yield return new WaitForSeconds(velFala);
        }

        isTyping = false;
        dialogoIndex++;
    }

    private void Update()
    {
        if (isDialogoAtivo && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            ProxLinha();
        }
    }

    public void FimDialogo()
    {
        StopAllCoroutines();
        isDialogoAtivo = false;
        dialogoAtivoPublico = false;
        isTyping = false;

        if (dialogoTxt != null) dialogoTxt.text = "";
        if (dialogoPanel != null) dialogoPanel.SetActive(false);
        if (sanidadeBar != null) sanidadeBar.SetActive(true);

        Debug.Log("✅ Diálogo finalizado.");
    }
}
