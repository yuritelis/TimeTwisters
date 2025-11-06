using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameSession instance;

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
}
