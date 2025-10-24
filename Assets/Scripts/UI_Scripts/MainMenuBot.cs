using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotController : MonoBehaviour
{
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject creditsScreen;

    public AudioManager aManager;

    private string nomeCena = "Alpha";

    private void Awake()
    {
        aManager = GameObject.FindFirstObjectByType<AudioManager>();
    }

    private void Start()
    {
        Object.DontDestroyOnLoad(gameObject);

        titleScreen.SetActive(true);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    public void BotPlay()
    {
        aManager.PlaySFX(aManager.botClick);
        SceneManager.LoadScene(nomeCena);
    }

    public void BotOptions()
    {
        aManager.PlaySFX(aManager.botClick);
        titleScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void BotCredits()
    {
        aManager.PlaySFX(aManager.botClick);
        titleScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void BotMenu()
    {
        aManager.PlaySFX(aManager.botClick);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

    public void BotSair()
    {
        aManager.PlaySFX(aManager.botClick);
        Application.Quit();
    }
}
