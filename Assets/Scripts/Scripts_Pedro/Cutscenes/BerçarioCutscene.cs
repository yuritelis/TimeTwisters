using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class CutsceneBerçario : MonoBehaviour
{
    public CameraSegue cam;
    public string playerTag = "Player";

    public MonoBehaviour playerController;
    private Rigidbody2D playerRb;

    public bool playOnlyOnce = true;
    private bool played = false;
    private Collider2D triggerCollider;
    private string saveKey;

    public Dialogo cutsceneDialogo;

    public float cameraMoveDuration = 0.9f;
    public AnimationCurve cameraEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public AudioClip musicaCutscene;

    public string cenaDestino;
    public string spawnPointDestino;

    private Transform focusProxy;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        if (cam == null)
            cam = Object.FindFirstObjectByType<CameraSegue>();

        focusProxy = new GameObject($"{name}_CameraProxy").transform;
        focusProxy.hideFlags = HideFlags.HideInHierarchy;

        saveKey = "Cutscene_" + gameObject.scene.name + "_" + gameObject.name;
        played = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (played && playOnlyOnce)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (played && playOnlyOnce) return;

        cam = FindAnyObjectByType<CameraSegue>();

        played = true;
        triggerCollider.enabled = false;

        if (playOnlyOnce)
        {
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();
        }

        StartCoroutine(RunCutscene(other.gameObject));
    }

    private IEnumerator RunCutscene(GameObject player)
    {
        if (playerController == null)
            playerController = player.GetComponent<PlayerController>();

        playerRb = player.GetComponent<Rigidbody2D>();
        if (playerController != null) playerController.enabled = false;
        if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

        DialogoManager.Instance.OnFalaIniciada = null;
        DialogoManager.Instance.OnFalaIniciada += HandleFalaIniciada;
        DialogoManager.Instance.StartDialogo(cutsceneDialogo);

        while (DialogoManager.Instance.dialogoAtivoPublico)
            yield return null;

        DialogoManager.Instance.OnFalaIniciada -= HandleFalaIniciada;

        cam.EndTemporaryFocus();

        if (AudioManager.instance != null && musicaCutscene != null)
            AudioManager.instance.TocarMusicaCutscene(musicaCutscene);

        if (playerController != null)
            playerController.enabled = true;

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null)
            combat.enabled = true;

        if (!string.IsNullOrEmpty(spawnPointDestino))
            PlayerPrefs.SetString("SpawnPoint", spawnPointDestino);
        else
            PlayerPrefs.DeleteKey("SpawnPoint");

        SceneManager.LoadScene(cenaDestino);
    }

    private IEnumerator SmoothFocusTo(Vector3 pos)
    {
        Vector3 start = focusProxy.position;
        Vector3 end = new(pos.x, pos.y, start.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / cameraMoveDuration;
            focusProxy.position = Vector3.Lerp(start, end, cameraEase.Evaluate(t));
            yield return null;
        }

        focusProxy.position = end;
    }

    private void HandleFalaIniciada(DialogoFalas fala)
    {
        cam.BeginTemporaryFocus(focusProxy);

        focusProxy.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            0f
        );

        if (fala.focoCamera != null)
            StartCoroutine(SmoothFocusTo(fala.focoCamera.position));
    }
}
