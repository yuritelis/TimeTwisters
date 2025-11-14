using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    private static GameSession instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 🔥 OUVINDO TODAS AS MUDANÇAS DE CENA
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Chamado SEMPRE que uma cena muda
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        string cena = newScene.name;

        if (cena == "TitleScreen" || cena == "DeathScreen")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
