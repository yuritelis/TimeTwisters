using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CutsceneMusicTrigger : MonoBehaviour
{
    public string playerTag = "Player";

    public string cutsceneObjectName;
    public AudioClip musicaAntesDaCutscene;

    private string saveKey;
    private bool alreadyApplied = false;

    private void Awake()
    {
        saveKey = "Cutscene_" + gameObject.scene.name + "_" + cutsceneObjectName;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (alreadyApplied) return;

        bool cutsceneJaFoi = PlayerPrefs.GetInt(saveKey, 0) == 1;
        if (cutsceneJaFoi) return;

        if (AudioManager.instance != null && musicaAntesDaCutscene != null)
        {
            AudioManager.instance.StopMusic();
            AudioManager.instance.TocarMusicaCutscene(musicaAntesDaCutscene);
        }

        alreadyApplied = true;
    }
}
