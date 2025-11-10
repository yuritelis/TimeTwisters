using UnityEngine;
using System.Collections;

public class DiningRoomCutscene : MonoBehaviour
{
    [Header("Referências")]
    public CameraSegue cam;
    public Transform vandalTarget;
    public string playerTag = "Player";

    [Header("Controle do Jogador")]
    public MonoBehaviour playerController; // arraste seu script de movimento
    private Rigidbody2D playerRb;

    [Header("Tempos")]
    [Tooltip("Tempo para a câmera 'chegar' ao vândalo")]
    public float cameraSettleTime = 1.2f;
    [Tooltip("Tempo que a câmera permanece focada no vândalo")]
    public float cameraHoldTime = 2.0f;
    [Tooltip("Pausa antes de devolver o controle")]
    public float afterReturnDelay = 0.5f;

    [Header("Execução")]
    public bool playOnlyOnce = true;
    private bool played = false;

    [Header("Inimigo (Vândalo)")]
    [Tooltip("Arraste aqui o inimigo desativado na cena")]
    public GameObject enemyObject;
    [Tooltip("Atraso em segundos para ele aparecer após a cutscene começar")]
    public float enemyAppearDelay = 0.7f;

    // ============================
    // TRIGGER PRINCIPAL
    // ============================
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"➡️ OnTriggerEnter2D chamado por: {other.name} (Tag: {other.tag})");

        if (played && playOnlyOnce)
        {
            Debug.Log("⚠️ Cutscene já foi executada antes, ignorando.");
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            Debug.Log($"🚫 Tag incorreta ({other.tag}), esperando {playerTag}");
            return;
        }

        Debug.Log("✅ Iniciando Coroutine da cutscene...");
        StartCoroutine(RunCutscene(other.gameObject));
    }

    // ============================
    // CUTSCENE
    // ============================
    private IEnumerator RunCutscene(GameObject player)
    {
        Debug.Log("🎬 CUTSCENE STARTADA!");
        played = true;

        // ---- Garantir Rigidbody e Controller ----
        if (playerRb == null)
            playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            Debug.Log("🧊 Player parado (velocity zerada).");
        }
        else
            Debug.LogWarning("⚠️ Player não tem Rigidbody2D.");

        if (playerController == null)
            playerController = player.GetComponent<MonoBehaviour>();

        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log("🧍‍♂️ Controle do jogador desativado.");
        }
        else
            Debug.LogWarning("⚠️ playerController está nulo!");

        // ---- Foco da Câmera ----
        if (cam != null && vandalTarget != null)
        {
            Debug.Log($"🎥 Focando câmera em: {vandalTarget.name}");
            cam.BeginTemporaryFocus(vandalTarget);
        }
        else
        {
            Debug.LogWarning("⚠️ Câmera ou VandalTarget estão nulos!");
        }

        // ---- Aparição do inimigo ----
        if (enemyObject != null)
        {
            Debug.Log($"⏳ Aguardando {enemyAppearDelay}s para ativar inimigo...");
            yield return new WaitForSeconds(enemyAppearDelay);

            enemyObject.SetActive(true);
            Debug.Log("💀 Inimigo ativado!");
        }
        else
        {
            Debug.LogWarning("⚠️ Nenhum inimigo atribuído em enemyObject!");
        }

        // ---- Duração da cutscene ----
        Debug.Log($"⏱️ Mantendo foco por {cameraSettleTime + cameraHoldTime}s...");
        yield return new WaitForSeconds(cameraSettleTime + cameraHoldTime);

        // ---- Voltar câmera ----
        if (cam != null)
        {
            cam.EndTemporaryFocus();
            Debug.Log("🎥 Câmera retornou ao jogador.");
        }

        // ---- Delay antes de liberar player ----
        yield return new WaitForSeconds(afterReturnDelay);

        // ---- Reativa o jogador ----
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("🕹️ Controle do jogador reativado!");
        }

        // ---- Marca como executado ----
        if (playOnlyOnce)
        {
            Debug.Log("🧩 Desativando trigger (one-shot).");
            gameObject.SetActive(false);
        }

        Debug.Log("✅ CUTSCENE FINALIZADA.");
    }
}
