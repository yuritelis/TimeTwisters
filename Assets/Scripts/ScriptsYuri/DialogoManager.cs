using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour//, IInteractable
{
    public static DialogoManager Instance;

    Dialogo dialogoData;

    public GameObject dialogoPanel;
    public GameObject sanidadeBar;
    public TextMeshProUGUI dialogoTxt, personagemNome;
    public Image personagemIcon;

    private int dialogoIndex;
    private bool isTyping, isDialogoAtivo = false;
    public float velFala = 1f;

    private void Start()
    {
        if(Instance == null)
            Instance = this;
    }

    public void StartDialogo(Dialogo dialogo)
    {
        Debug.Log("Chamou StartDialogo");

        isDialogoAtivo = true;
        dialogoIndex = 0;

        dialogoPanel.SetActive(true);
        sanidadeBar.SetActive(false);

        ProxLinha();
    }

    public void ProxLinha()
    {
        if (dialogoIndex++ < dialogoData.dialogoFalas.Count)
        {
            dialogoData.dialogoFalas.RemoveAt(0);
        }

        DialogoFalas falaAtual;

        personagemIcon.sprite = falaAtual.personagem.portrait;
        personagemNome.text = falaAtual.personagem.nome;

        StartCoroutine(TypeLine(falaAtual));
    }

    IEnumerator TypeLine(DialogoFalas fala)
    {
        isTyping = true;
        dialogoTxt.SetText("");

        foreach (char letter in fala.fala.ToCharArray())
        {
            dialogoTxt.text += letter;
            //AudioManager.instance.PlaySFX(PersoInfos.somVoz);
            yield return new WaitForSeconds(velFala);
        }

        isTyping = false;
    }

    public void FimDialogo()
    {
        StopAllCoroutines();
        isDialogoAtivo = false;
        dialogoTxt.SetText("");
        dialogoPanel.SetActive(false);
        sanidadeBar.SetActive(true);
    }
}