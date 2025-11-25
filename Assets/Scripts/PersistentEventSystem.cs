using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PersistentEventSystem : MonoBehaviour
{
    private static PersistentEventSystem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        DisableOtherEventSystems();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DisableOtherEventSystems();
    }

    private void DisableOtherEventSystems()
    {
        EventSystem[] systems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        foreach (var es in systems)
        {
            if (es.gameObject != this.gameObject)
            {
                es.gameObject.SetActive(false);
            }
        }
    }
}
