using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogo dialogoData;
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
        if(dialogoData == null || (PauseGame.Instance.isPaused && !isDialogoAtivo))
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

        nomeTxt.SetText(dialogoData.nomeNpc);
        portraitImg.sprite = dialogoData.npcPortrait;

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

        foreach(char letter in dialogoData.linhasDialogo[dialogoIndex])
        {
            dialogoTxt.text += letter;
            yield return new WaitForSeconds(dialogoData.velFala);
        }

        isTyping = false;

        if(dialogoData.autoProgressLine.Length > dialogoIndex && dialogoData.autoProgressLine[dialogoIndex])
        {
            yield return new WaitForSeconds(dialogoData.autoProgressDelay);
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
