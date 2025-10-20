using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject optionsScreen;

    public PlayerInput playerInput;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            { 
                Pause(); 
            }
            else
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        isPaused = true;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            // Desativa TODOS os scripts e componentes
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) // Não desativa o PauseGame
                    script.enabled = false;
            }

            // Desativa Animator
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            // Congela física
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }

        if (playerInput != null)
        {
            playerInput.enabled = false;
            playerInput.actions.Disable();
        }
    }

    private void Resume()
    {
        Time.timeScale = 1.0f;
        pauseScreen.SetActive(false);
        isPaused = false;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            // Desativa TODOS os scripts e componentes
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this) // Não desativa o PauseGame
                    script.enabled = true;
            }

            // Desativa Animator
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.enabled = true;

            // Congela física
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }

        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerInput.actions.Enable();
        }
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

        Resume();
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