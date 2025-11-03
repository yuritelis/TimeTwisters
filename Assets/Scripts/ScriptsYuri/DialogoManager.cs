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

    private Queue<DialogoFalas> falas = new Queue<DialogoFalas>();

    //private int dialogoIndex;
    private bool isTyping, isDialogoAtivo = false;
    public float velFala = 0.5f;

    private void Start()
    {
        if(Instance == null)
            Instance = this;
    }

    /*public bool CanInteract()
    {
        return !isDialogoAtivo;
    }

    public void Interact()
    {
        if (dialogoData == null || (PauseController.IsGamePaused && !isDialogoAtivo))
        {
            return;
        }

        if (isDialogoAtivo && falas.Count > 1)
        {
            ProxLinha();
        }
        else
        {
            StartDialogo(dialogoData);
        }
    }*/

    public void StartDialogo(Dialogo dialogo)
    {
        Debug.Log("Chamou StartDialogo");

        isDialogoAtivo = true;
        //dialogoIndex = 0;

        foreach (DialogoFalas dialogoFala in dialogo.dialogoFalas)
        {
            falas.Enqueue(dialogoFala);
        }

        dialogoPanel.SetActive(true);
        sanidadeBar.SetActive(false);

        ProxLinha();
    }

    public void ProxLinha()
    {
        if (falas.Count == 1)
        {
            FimDialogo();
            return;
        }

        DialogoFalas falaAtual = falas.Dequeue();

        personagemIcon.sprite = falaAtual.personagem.portrait;
        personagemNome.text = falaAtual.personagem.nome;

        StopAllCoroutines();
        StartCoroutine(TypeLine(falaAtual));

        if (isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0)))
        {
            StopAllCoroutines();
            dialogoTxt.SetText(falaAtual.fala);
            isTyping = false;
        }
        else
        {
            StartCoroutine(TypeLine(falaAtual));
        }
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

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (isTyping)
                {
                    dialogoTxt.text = fala.fala;
                }
                else
                {
                    ProxLinha();
                }
            }
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