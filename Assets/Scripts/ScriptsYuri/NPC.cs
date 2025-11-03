using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public Dialogo dialogoData;
    public GameObject dialogoPanel;
    public TMP_Text dialogoTxt, nomeTxt;
    public Image portraitImg;
    public GameObject sanidadeBar;

    private int dialogoIndex;
    private bool isTyping, isDialogoAtivo = false;

    public bool CanInteract()
    {
        return !isDialogoAtivo;
    }

    public void Interact()
    {
        if (dialogoData == null || (PauseController.IsGamePaused && !isDialogoAtivo))
        {
            return;
        }

        if (isDialogoAtivo)
        {
            ProxLinha();
        }
        else
        {
            StartDialogo();
        }
    }

    void StartDialogo()
    {
        isDialogoAtivo = true;
        dialogoIndex = 0;

        nomeTxt.SetText(dialogoData.personagem.nome);
        portraitImg.sprite = dialogoData.personagem.portrait;

        dialogoPanel.SetActive(true);
        sanidadeBar.SetActive(false);

        StartCoroutine(TypeLine());
    }

    void ProxLinha()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogoTxt.SetText(dialogoData.linhasDialogo[dialogoIndex]);
            isTyping = false;
        }
        else if (++dialogoIndex < dialogoData.linhasDialogo.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            FimDialogo();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogoTxt.SetText("");

        foreach (char letter in dialogoData.linhasDialogo[dialogoIndex])
        {
            dialogoTxt.text += letter;
            AudioManager.instance.PlaySFX(PersoInfos.somVoz);
            yield return new WaitForSeconds(dialogoData.velFala);
        }

        isTyping = false;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ProxLinha();
        }
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