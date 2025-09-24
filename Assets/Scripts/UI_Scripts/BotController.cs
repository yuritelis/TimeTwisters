using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotController : MonoBehaviour
{
    public void BotPlay()
    {
        SceneManager.LoadScene("Teste01");
    }

    public void BotOptions()
    {
        SceneManager.LoadScene("OptionsScreen");
    }

    public void BotCredits()
    {
        SceneManager.LoadScene("CreditsScreen");
    }

    public void BotMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BotSair()
    {
        Application.Quit();
    }
}
