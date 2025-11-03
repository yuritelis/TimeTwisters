using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DialogoTrigger : MonoBehaviour
{
    public Dialogo dialogo;

    public void TriggerDialogo()
    {
        Debug.Log("Chamou TriggerDialogo");
        DialogoManager.Instance.StartDialogo(dialogo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter rsrs");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Achou a PORRA do player");
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Apertou E");
                TriggerDialogo();
            }
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