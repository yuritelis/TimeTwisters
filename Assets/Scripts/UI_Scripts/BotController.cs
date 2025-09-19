using UnityEngine;
using UnityEngine.SceneManagement;

public class BotController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void BotPlay()
    {
        SceneManager.LoadScene("");
    }

    void BotOptions()
    {
        SceneManager.LoadScene("MenuOptions");
    }

    void BotCredits()
    {
        SceneManager.LoadScene("MenuCredits");
    }

    void BotSair()
    {
        Application.Quit();
    }
}
