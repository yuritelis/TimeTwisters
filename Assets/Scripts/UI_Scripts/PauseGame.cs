using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gObject;
    private bool isPaused = false;

    private void Awake()
    {
        Object.DontDestroyOnLoad(gObject);
    }

    void Start()
    {
        pauseScreen.SetActive(false);
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
    }

    public void BotMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BotResume()
    {
        pauseScreen.SetActive(false);
    }
}
