using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class DiningRoomCutscene : MonoBehaviour
{
    [Header("Referências")]
    public CameraSegue cam;
    public string playerTag = "Player";

    [Header("Controle do Jogador")]
    public MonoBehaviour playerController;
    private Rigidbody2D playerRb;

    [Header("Execução")]
    public bool playOnlyOnce = true;
    private bool played = false;
    private Collider2D triggerCollider;
    private string saveKey;

    [Header("Inimigo (Vândalo)")]
    public GameObject enemyObject;
    public float enemyAppearDelay = 0.7f;
    public List<MonoBehaviour> enemyAIScripts = new List<MonoBehaviour>();

    [Header("Câmera / Foco")]
    [Tooltip("Lista de pontos onde a câmera passará antes de voltar ao player.")]
    public List<Transform> cameraFocusPoints = new List<Transform>();
    [Tooltip("Tempo de transição entre pontos de câmera.")]
    public float cameraMoveDuration = 1.2f;
    [Tooltip("Tempo que a câmera permanece em cada ponto.")]
    public float cameraHoldTime = 2f;

    [Header("Diálogo da Cutscene")]
    [Tooltip("Diálogo a ser reproduzido durante a cutscene.")]
    public Dialogo cutsceneDialogo;

    [Header("Delay final antes de devolver controle")]
    public float afterReturnDelay = 0.5f;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        if (cam == null)
            cam = Object.FindFirstObjectByType<CameraSegue>();

        saveKey = "Cutscene_" + gameObject.scene.name + "_" + gameObject.name;
        played = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (played && playOnlyOnce)
        {
            gameObject.SetActive(false);
            return;
        }

        if (enemyObject != null)
        {
            foreach (var ai in enemyAIScripts)
                if (ai != null)
                    ai.enabled = false;

            enemyObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(EnsureCameraReference());
    }

    private IEnumerator EnsureCameraReference()
    {
        yield return null;
        if (cam == null)
            cam = Object.FindFirstObjectByType<CameraSegue>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (played && playOnlyOnce) return;

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
        yield return null;

        // === Travar jogador ===
        if (playerController == null)
            playerController = player.GetComponent<PlayerController>();
        playerRb = player.GetComponent<Rigidbody2D>();

        if (playerController != null) playerController.enabled = false;
        if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

        DisableEnemyAI();

        // === Sequência da câmera ===
        if (cam != null && cameraFocusPoints.Count > 0)
        {
            foreach (Transform point in cameraFocusPoints)
            {
                cam.BeginTemporaryFocus(point);
                yield return new WaitForSeconds(cameraMoveDuration);

                yield return new WaitForSeconds(cameraHoldTime);
            }
        }

        // === Ativar inimigo ===
        if (enemyObject != null)
        {
            yield return new WaitForSeconds(enemyAppearDelay);
            enemyObject.SetActive(true);
        }

        // === Diálogo da cutscene ===
        if (cutsceneDialogo != null && DialogoManager.Instance != null)
        {
            DialogoManager.Instance.StartDialogo(cutsceneDialogo);

            while (DialogoManager.Instance.dialogoAtivoPublico)
                yield return null;
        }

        // === Retornar câmera ===
        if (cam != null)
        {
            cam.EndTemporaryFocus();
        }

        // === Retornar controle ===
        yield return new WaitForSeconds(afterReturnDelay);

        if (playerController != null)
            playerController.enabled = true;

        EnableEnemyAI();

        if (playOnlyOnce)
            gameObject.SetActive(false);
    }

    private void DisableEnemyAI()
    {
        foreach (var script in enemyAIScripts)
        {
            if (script != null)
                script.enabled = false;
        }
    }

    private void EnableEnemyAI()
    {
        foreach (var script in enemyAIScripts)
        {
            if (script != null)
                script.enabled = true;
        }
    }
}
