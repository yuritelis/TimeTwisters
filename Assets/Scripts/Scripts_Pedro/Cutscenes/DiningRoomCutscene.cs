using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class DiningRoomCutscene : MonoBehaviour
{
    [Header("Referências")]
    public CameraSegue cam;
    public Transform vandalTarget;
    public string playerTag = "Player";

    [Header("Controle do Jogador")]
    public MonoBehaviour playerController;
    private Rigidbody2D playerRb;

    [Header("Tempos")]
    public float cameraSettleTime = 1.2f;
    public float cameraHoldTime = 2.0f;
    public float afterReturnDelay = 0.5f;

    [Header("Execução")]
    public bool playOnlyOnce = true;
    private bool played = false;
    private Collider2D triggerCollider;
    private string saveKey; // chave única pra salvar o estado

    [Header("Inimigo (Vândalo)")]
    [Tooltip("Arraste aqui o inimigo (deve começar desativado ou será desativado automaticamente no Awake)")]
    public GameObject enemyObject;
    public float enemyAppearDelay = 0.7f;
    public List<MonoBehaviour> enemyAIScripts = new List<MonoBehaviour>();

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        if (cam == null)
            cam = Object.FindFirstObjectByType<CameraSegue>();

        // 🔑 Cria chave única baseada na cena + nome
        saveKey = "Cutscene_" + gameObject.scene.name + "_" + gameObject.name;
        played = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (played && playOnlyOnce)
        {
            // já foi executada antes — desativa o trigger de vez
            gameObject.SetActive(false);
            Debug.Log($"⛔ Cutscene {name} já foi executada antes, desativando permanentemente.");
            return;
        }

        // 🧩 Garante que o inimigo comece desativado SEMPRE
        if (enemyObject != null)
        {
            foreach (var ai in enemyAIScripts)
                if (ai != null)
                    ai.enabled = false;

            if (enemyObject.activeSelf)
            {
                enemyObject.SetActive(false);
                Debug.Log($"🔒 Inimigo '{enemyObject.name}' forçado a iniciar desativado no Awake().");
            }
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
        {
            cam = Object.FindFirstObjectByType<CameraSegue>();
            if (cam != null)
                Debug.Log($"🎥 Cutscene {name}: CameraSegue reassociado automaticamente ({cam.name}).");
            else
                Debug.LogWarning($"⚠️ Cutscene {name}: nenhum CameraSegue encontrado na cena!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (played && playOnlyOnce) return;

        played = true;

        if (triggerCollider != null)
            triggerCollider.enabled = false;

        // 🔐 Salva o estado de que a cutscene já foi tocada
        if (playOnlyOnce)
        {
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();
        }

        Debug.Log($"🎬 Cutscene disparada por {other.name}");
        StartCoroutine(RunCutscene(other.gameObject));
    }

    private IEnumerator RunCutscene(GameObject player)
    {
        yield return null;

        DisableEnemyAI();

        if (playerController == null)
            playerController = player.GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("❌ Cutscene cancelada: playerController não encontrado!");
            yield break;
        }

        playerController.enabled = false;
        playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        if (cam != null && vandalTarget != null)
        {
            cam.BeginTemporaryFocus(vandalTarget);
            Debug.Log($"🎥 Focando câmera em {vandalTarget.name}...");
        }

        yield return new WaitForSeconds(enemyAppearDelay);

        if (enemyObject != null && !enemyObject.activeSelf)
        {
            enemyObject.SetActive(true);
            Debug.Log("👁️ Inimigo ativado (IA ainda desativada).");
        }

        yield return new WaitForSeconds(cameraSettleTime + cameraHoldTime);

        if (cam != null)
        {
            cam.EndTemporaryFocus();
            Debug.Log("🎥 Câmera voltou ao jogador.");
        }

        yield return new WaitForSeconds(afterReturnDelay);

        playerController.enabled = true;

        EnableEnemyAI();

        if (playOnlyOnce)
            gameObject.SetActive(false);

        Debug.Log("✅ Cutscene finalizada com sucesso.");
    }

    private void DisableEnemyAI()
    {
        foreach (var script in enemyAIScripts)
        {
            if (script != null)
            {
                script.enabled = false;
                Debug.Log($"🧠 Desativando IA: {script.GetType().Name}");
            }
        }
    }

    private void EnableEnemyAI()
    {
        foreach (var script in enemyAIScripts)
        {
            if (script != null)
            {
                script.enabled = true;
                Debug.Log($"⚔️ Reativando IA: {script.GetType().Name}");
            }
        }
    }
}
