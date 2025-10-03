using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject optionsScreen;
    private bool isPaused = false;

    private void Awake()
    {
        Object.DontDestroyOnLoad(gameObject);
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
    }

    public void BotMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BotResume()
    {
        pauseScreen.SetActive(false);
        optionsScreen.SetActive(false);
    }

    public void BotOpcoes()
    {
        pauseScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }
}
