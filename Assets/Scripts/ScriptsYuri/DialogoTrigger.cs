using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DialogoTrigger : MonoBehaviour
{
    public Dialogo dialogo;

    private bool playerInRange;
    private GameObject player;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Apertou E");
            TriggerDialogo();
        }
    }

    public void TriggerDialogo()
    {
        Debug.Log("Chamou TriggerDialogo");
        DialogoManager.Instance.StartDialogo(dialogo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter: player entrou no range");
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit: player saiu do range");
        if (collision.CompareTag("Player"))
        {
            player = null;
            playerInRange = false;

            DialogoManager.Instance.FimDialogo();
        }
    }
}

[System.Serializable]
public class PersoInfos
{
    public string nome;
    public Sprite portrait;
    public static AudioClip somVoz;
}

[System.Serializable]
public class DialogoFalas
{
    public PersoInfos personagem;

    [TextArea(3, 10)]
    public string fala;
}

[System.Serializable]
public class Dialogo
{
    public List<DialogoFalas> dialogoFalas;
}