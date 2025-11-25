using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public GameObject[] objetosNaoParaTitleScreen;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        bool isInactive = newScene.name == "TitleScreen" || newScene.name == "DeathScreen";

        foreach (var obj in objetosNaoParaTitleScreen)
        {
            if (obj != null)
            {
                foreach (var child in obj.GetComponentsInChildren<Transform>(true))
                {
                    string name = child.gameObject.name;
                    if (name == "Progress" || name == "pauseScreen" || name == "DialogoPanel")
                        continue;

                    child.gameObject.SetActive(!isInactive);
                }
            }
        }
    }
}
