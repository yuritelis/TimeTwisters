using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotController : MonoBehaviour
{
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] GameObject gObject;

    private void Start()
    {
        Object.DontDestroyOnLoad(gObject);

        titleScreen.SetActive(true);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    public void BotPlay()
    {
        SceneManager.LoadScene("Prototipo");
    }

    public void BotOptions()
    {
        titleScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void BotCredits()
    {
        titleScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void BotMenu()
    {
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

    public void BotSair()
    {
        Application.Quit();
    }
}
