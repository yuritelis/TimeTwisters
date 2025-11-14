using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    private static GameSession instance;

    private string cena;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Evita duplicar ao voltar pra cena anterior
        }
    }

    private void Start()
    {
        if (cena == "DeathScreen")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        cena = SceneManager.GetActiveScene().name;
    }
}
