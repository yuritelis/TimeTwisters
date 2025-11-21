using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject inventarioScreen;
    [SerializeField] GameObject sanidadeBar;

    private AudioManager aManager;
    private TimelineUI timelineUI;

    private bool inventarioAberto = false;

    private void Awake()
    {
        aManager = FindFirstObjectByType<AudioManager>();
        timelineUI = FindFirstObjectByType<TimelineUI>();

        PauseController.SetPause(false);
        Time.timeScale = 1f;

        if (pauseScreen != null && pauseScreen.activeSelf)
            pauseScreen.SetActive(false);

        if (inventarioScreen != null)
            inventarioScreen.SetActive(false);

        if (sanidadeBar != null)
            sanidadeBar.SetActive(true);
    }

    private void Update()
    {
        if (!this.isActiveAndEnabled)
            Debug.LogWarning("⚠ PauseGame está em um objeto DESATIVADO! Mova-o para um GameObject sempre ativo.");

        if (TimelineUI.isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                timelineUI.Close();

            return;
        }

        if (inventarioAberto)
        {
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
            {
                inventarioScreen.SetActive(false);
                inventarioAberto = false;
                PauseController.SetPause(false);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventarioScreen.SetActive(true);
            inventarioAberto = true;
            PauseController.SetPause(true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PauseController.IsGamePaused)
                Pause();
            else
                Resume();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        sanidadeBar.SetActive(false);
        PauseController.SetPause(true);
    }

    private void Resume()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        sanidadeBar.SetActive(true);
        PauseController.SetPause(false);
    }

    public void BotMenu()
    {
        if (aManager != null)
            aManager.PlaySFX(aManager.botClick);

        PauseController.SetPause(false);
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;

        SceneManager.LoadScene("TitleScreen");
    }

    public void BotResume()
    {
        if (aManager != null)
            aManager.PlaySFX(aManager.botClick);

        Resume();
    }

    public void BotVoltar()
    {
        if (aManager != null)
            aManager.PlaySFX(aManager.botClick);

        pauseScreen.SetActive(true);
    }
}
