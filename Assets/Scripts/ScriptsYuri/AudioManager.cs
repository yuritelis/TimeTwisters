using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("> Audio Source <")]
    [SerializeField] AudioSource fonteMus;
    [SerializeField] AudioSource fonteSfx;

    [Header("> Audio Clip <")]
    [SerializeField] public AudioClip menuBgm;
    [SerializeField] public AudioClip presBgm;
    [SerializeField] public AudioClip pasBgm;
    [SerializeField] public AudioClip futBgm;
    [SerializeField] public AudioClip botClick;
    [SerializeField] public AudioClip interact;

    [Header("> TimeLine <")]
    [SerializeField] GameObject passado;
    [SerializeField] GameObject presente;
    [SerializeField] GameObject futuro;

    private string lastScene;
    private string currentScene;

    private void Awake()
    {
        lastScene = SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        Object.DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != lastScene)
        {
            lastScene = currentScene;
            ChangeMusic();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        fonteSfx.PlayOneShot(clip);
    }

    public void PlayMus(AudioClip clip)
    {
        fonteMus.Stop();
        fonteMus.PlayOneShot(clip);
    }

    void ChangeMusic()
    {
        if (lastScene == "MainMenu")
        {
            PlayMus(menuBgm);
        }
        else if (lastScene == "teste_yuri" && passado.activeSelf)
        {
            PlayMus(pasBgm);
        }
        else if (lastScene == "teste_yuri" && presente.activeSelf)
        {
            PlayMus(presBgm);
        }
        else if (lastScene == "teste_yuri" && futuro.activeSelf)
        {
            PlayMus(futBgm);
        }
    }

    public void PlayMusic(bool play)
    {
        if (play)
        {
            if (!fonteMus.isPlaying)
            {
                ChangeMusic();
            }
        }
        else
        {
            if (fonteMus.isPlaying)
            {

            }
            fonteMus.Stop();
        }
    }
}