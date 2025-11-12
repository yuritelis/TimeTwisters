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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        nearbyInteractable = null;
        nearbyTimeTravel = null;

        Debug.Log($"🌍 Cena carregada: {scene.name}. Resetando interações antigas.");
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (nearbyTimeTravel != null && !isWaitingForNewUI)
            {
                StartCoroutine(OpenTimelineUISafely());
                return;
            }

            if (nearbyInteractable != null)
            {
                nearbyInteractable.Interact();
            }
        }
    }

    private IEnumerator OpenTimelineUISafely()
    {
        isWaitingForNewUI = true;

        yield return null;

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
            if (uiAtual != null && uiAtual.gameObject != null)
                uiAtual.Open(null);
        }

        isWaitingForNewUI = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            nearbyInteractable = interactable;

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
