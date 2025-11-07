using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject sanidadeBar;

    AudioManager aManager;
    TimelineUI timelineUI;
    TimeTravelTilemap timeline;

    private void Awake()
    {
        aManager = FindFirstObjectByType<AudioManager>();
        timelineUI = FindFirstObjectByType<TimelineUI>();

        PauseController.SetPause(false);
        
        Time.timeScale = 1.0f;

        sanidadeBar.SetActive(true);
    }

    void Start()
    {
        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if(TimelineUI.isPaused != true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PauseController.IsGamePaused != true)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timelineUI.Close();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        PauseController.SetPause(true);
        sanidadeBar.SetActive(false);
    }

    private void Resume()
    {
        Time.timeScale = 1.0f;
        pauseScreen.SetActive(false);
        PauseController.SetPause(false);
        sanidadeBar.SetActive(true);
    }

    public void BotMenu()
    {
        if (aManager != null)
        {
            aManager.PlaySFX(aManager.botClick);
        }

        PauseController.SetPause(false);

        SceneManager.LoadScene("TitleScreen");
    }

    public void BotResume()
    {
        if (aManager != null)
        {
            aManager.PlaySFX(aManager.botClick);
        }

        Resume();
    }

    public void BotVoltar()
    {
        if (aManager != null)
        {
            aManager.PlaySFX(aManager.botClick);
        }

        pauseScreen.SetActive(true);
    }
}