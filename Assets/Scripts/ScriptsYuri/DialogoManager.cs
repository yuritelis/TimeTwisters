using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance;

    Dialogo dialogoData;

    public GameObject dialogoPanel;
    public GameObject sanidadeBar;
    public TextMeshProUGUI dialogoTxt, personagemNome;
    public Image personagemIcon;

    private int dialogoIndex;
    private bool isTyping, isDialogoAtivo, fimFala = false;
    public float velFala = 1f;

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
        Debug.Log("Chamou StartDialogo");

        if (dialogo == null || dialogo.dialogoFalas.Count == 0)
        {
            Debug.LogError("Dialogo data está vazio ou nulo!");
            return;
        }

        isDialogoAtivo = true;
        dialogoIndex = 0;
        dialogoData = dialogo;

        dialogoPanel.SetActive(true);
        if (sanidadeBar != null) sanidadeBar.SetActive(false);

        ProxLinha();
    }

    public void ProxLinha()
    {
        if (!isDialogoAtivo || dialogoData == null) return;

        if (fimFala)
        {
            dialogoIndex += dialogoData.dialogoFalas.Count - 1;
        }

        if (dialogoIndex >= dialogoData.dialogoFalas.Count)
        {
            FimDialogo();
            return;
        }

        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
            dialogoTxt.text = dialogoData.dialogoFalas[dialogoIndex].fala;
            dialogoIndex++;
            return;
        }

        var falaAtual = dialogoData.dialogoFalas[dialogoIndex];

        if (personagemNome != null)
            personagemNome.text = falaAtual.personagem.nome;

        if (personagemIcon != null && falaAtual.personagem.portrait != null)
            personagemIcon.sprite = falaAtual.personagem.portrait;

        StartCoroutine(TypeLine(falaAtual.fala));
    }

    IEnumerator TypeLine(string fala)
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
        if (isDialogoAtivo && Input.GetKeyDown(KeyCode.Space))
        {
            ProxLinha();
        }
    }

    public void FimDialogo()
    {
        StopAllCoroutines();
        isDialogoAtivo = false;
        isTyping = false;
        fimFala = true;

        if (dialogoTxt != null) dialogoTxt.text = "";
        if (dialogoPanel != null) dialogoPanel.SetActive(false);
        if (sanidadeBar != null) sanidadeBar.SetActive(true);

        Debug.Log("Dialogo finalizado");
    }
}