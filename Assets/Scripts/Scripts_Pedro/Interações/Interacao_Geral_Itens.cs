using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interacao_Geral_Itens : MonoBehaviour, IInteractable
{
    [Header("Diálogo a ser exibido ao interagir")]
    public Dialogo dialogo;

    private bool jaInteragiu = false;
    private GameObject player;

    public bool CanInteract()
    {
        return !jaInteragiu && dialogo != null;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        jaInteragiu = true;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        TravarJogador(true);

        if (DialogoManager.Instance == null)
        {
            Debug.LogError("⚠️ Nenhum DialogoManager encontrado na cena!");
            TravarJogador(false);
            return;
        }

        // ================================ //
        // 🎵 SOM DE INTERAÇÃO (igual SceneTransition)
        // ================================ //
        if (dialogo != null && dialogo.dialogoFalas != null && dialogo.dialogoFalas.Count > 0)
        {
            // tenta achar o primeiro som configurado em qualquer fala
            AudioClip som = null;

            foreach (var fala in dialogo.dialogoFalas)
            {
                if (fala.sfxAposFala != null)
                {
                    som = fala.sfxAposFala;
                    break;
                }
            }

            if (som != null)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySFX(som);
                else
                    AudioSource.PlayClipAtPoint(som, transform.position);
            }
        }

        // ================================ //
        // 💬 INICIAR DIÁLOGO
        // ================================ //
        DialogoManager.Instance.StartDialogo(dialogo);

        StartCoroutine(EsperarDialogoTerminar());
    }

    private IEnumerator EsperarDialogoTerminar()
    {
        while (DialogoManager.Instance != null && DialogoManager.Instance.dialogoAtivoPublico)
            yield return null;

        TravarJogador(false);
    }

    private void TravarJogador(bool estado)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        var combat = player.GetComponent<Player_Combat>();

        if (controller != null)
            controller.canMove = !estado;

        if (combat != null)
            combat.enabled = !estado;
    }
}
