using UnityEngine;

public class ObjetosInterativos : MonoBehaviour
{
    private bool playerPerto = false;
    public GameObject interacaoKey;

    void Update()
    {
        interacaoKey.SetActive(playerPerto);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PLayer"))
        {
            playerPerto = false;
        }
    }
}
