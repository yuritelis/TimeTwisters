using UnityEngine;
using UnityEngine.UI;

public class JournalInteractable : MonoBehaviour, IInteractable
{
    [Header("UI da Carta")]
    public CanvasGroup cartaUI;
    public KeyCode closeKey = KeyCode.E;

    private bool cartaAberta = false;

    public bool CanInteract() => !cartaAberta;

    public void Interact()
    {
        if (cartaAberta) return;

        cartaUI.gameObject.SetActive(true);
        StartCoroutine(FadeCarta(0, 1));
        cartaAberta = true;

        Time.timeScale = 0f; // pausa o jogo
    }

    void Update()
    {
        if (cartaAberta && (Input.GetKeyDown(closeKey) || Input.GetMouseButtonDown(0)))
        {
            StartCoroutine(FecharCarta());
        }
    }

    private System.Collections.IEnumerator FecharCarta()
    {
        yield return StartCoroutine(FadeCarta(1, 0));
        cartaUI.gameObject.SetActive(false);
        cartaAberta = false;
        Time.timeScale = 1f; // retoma o jogo
    }

    private System.Collections.IEnumerator FadeCarta(float start, float end)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 2f;
            cartaUI.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }
    }
}
