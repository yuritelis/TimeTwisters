using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject optionsScreen;

    private bool isPaused = false;

    AudioManager aManager;

    private void Awake()
    {
        Object.DontDestroyOnLoad(gameObject);

        aManager = FindFirstObjectByType<AudioManager>();
    }

    void Start()
    {
        pauseScreen.SetActive(false);
        optionsScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Pause();
        }
        else
        {
            if (isPaused)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    Resume();
                }
            }
        }
    }

    private void Pause()
    {
        pauseScreen.SetActive(true);

        isPaused = true;
    }

    private void Resume()
    {
        pauseScreen.SetActive(false);
        optionsScreen.SetActive(false);

        isPaused = false;
    }

    public void BotMenu()
    {
        if(aManager != null)
        {
            aManager.PlaySFX(aManager.botClick);
        }

        SceneManager.LoadScene("TitleScreen");
    }

    public void BotResume()
    {
        if (aManager != null)
        {
            aManager.PlaySFX(aManager.botClick);
        }

        pauseScreen.SetActive(false);
        optionsScreen.SetActive(false);

        isPaused = false;
    }

    public void BotOpcoes()
    {
        if (aManager != null)
        {
            aManager.PlaySFX(aManager.botClick);
        }

        pauseScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void BotVoltar()
    {
        if (aManager != null)
        {
            aManager.PlaySFX(aManager.botClick);
        }

        pauseScreen.SetActive(true);
        optionsScreen.SetActive(false);
    }
}
