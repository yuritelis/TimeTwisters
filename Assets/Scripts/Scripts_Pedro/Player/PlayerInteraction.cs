using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interação")]
    public KeyCode interactKey = KeyCode.E;

    private IInteractable nearbyInteractable;
    private TimeTravelMarker nearbyTimeTravel;
    private bool isWaitingForNewUI = false;

    void OnEnable()
    {
        // 🔹 Se inscreve no evento de troca de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 🔹 Remove inscrição ao desativar
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 🔸 Chamado automaticamente sempre que uma nova cena é carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🔹 Limpa qualquer referência que veio da cena anterior
        nearbyInteractable = null;
        nearbyTimeTravel = null;

        // 🔹 Apenas debug (opcional)
        Debug.Log($"🌍 Cena carregada: {scene.name}. Resetando interações antigas.");
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // 🔹 Interação temporal (vitrola, espelho, etc.)
            if (nearbyTimeTravel != null && !isWaitingForNewUI)
            {
                StartCoroutine(OpenTimelineUISafely());
                return;
            }

            // 🔹 Interação normal
            if (nearbyInteractable != null)
            {
                nearbyInteractable.Interact();
            }
        }
    }

    private IEnumerator OpenTimelineUISafely()
    {
        isWaitingForNewUI = true;

        // Espera um frame para garantir que a cena anterior terminou de descarregar
        yield return null;

        // 🔹 Procura a UI mais recente
        TimelineUI uiAtual = null;
        float timeout = 2f;
        float elapsed = 0f;

        while (uiAtual == null && elapsed < timeout)
        {
            uiAtual = FindFirstObjectByType<TimelineUI>();
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (uiAtual == null)
        {
            Debug.LogWarning("⚠️ Nenhuma TimelineUI encontrada na nova cena.");
        }
        else if (uiAtual.gameObject == null)
        {
            Debug.LogWarning("⚠️ TimelineUI foi destruída antes de ser usada!");
        }
        else
        {
            // 🔹 Confirma se o objeto ainda é válido antes de abrir
            if (uiAtual != null && uiAtual.gameObject != null)
                uiAtual.Open(null);
        }

        isWaitingForNewUI = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta interações normais
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            nearbyInteractable = interactable;

        // Detecta objetos de viagem no tempo
        TimeTravelMarker timeTravel = other.GetComponent<TimeTravelMarker>();
        if (timeTravel != null)
            nearbyTimeTravel = timeTravel;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable == nearbyInteractable)
            nearbyInteractable = null;

        TimeTravelMarker timeTravel = other.GetComponent<TimeTravelMarker>();
        if (timeTravel == nearbyTimeTravel)
            nearbyTimeTravel = null;
    }
}
