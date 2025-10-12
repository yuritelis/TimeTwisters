using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("> Audio Source <")]
    [SerializeField] AudioSource musSource;
    [SerializeField] AudioSource sfxSource;

    [Header("> Audio Clip <")]
    [SerializeField] public AudioClip bgMus;
    [SerializeField] public AudioClip botClick;
    [SerializeField] public AudioClip interact;

    void Start()
    {
        Object.DontDestroyOnLoad(gameObject);
        musSource.clip = bgMus;
        musSource.Play();
    }

    void Update()
    {
        
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
