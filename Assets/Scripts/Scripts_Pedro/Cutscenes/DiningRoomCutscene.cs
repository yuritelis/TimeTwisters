using UnityEngine;
using System.Collections;

public class DiningRoomCutscene : MonoBehaviour
{
    [Header("Referências")]
    public Transform vandal;          // inimigo/vândalo que a câmera vai focar
    public CameraSegue cameraSegue;   // sua câmera de seguir
    public float moveSpeed = 2f;      // velocidade de movimento da câmera
    public float holdTime = 2f;       // tempo que ela foca o vândalo
    public GameObject enemyToActivate; // inimigo que será ativado no fim

    private bool played = false;
    private GameObject player;
    private PlayerController playerController;
    private Transform cameraTransform;
    private Transform originalTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (played) return;
        if (!other.CompareTag("Player")) return;

        played = true;
        player = other.gameObject;
        playerController = player.GetComponent<PlayerController>();
        cameraTransform = cameraSegue.transform;
        originalTarget = cameraSegue.player;

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // 🔒 trava o player
        if (playerController != null)
            playerController.canMove = false;

        // 🚫 desliga o follow da câmera
        cameraSegue.player = null;

        Vector3 startPos = cameraTransform.position;
        Vector3 targetPos = new Vector3(vandal.position.x, vandal.position.y, cameraSegue.cameraZ);

        // 🎥 move até o vândalo
        yield return StartCoroutine(MoveCamera(startPos, targetPos));

        // 👁️ espera um pouco olhando pra ele
        yield return new WaitForSeconds(holdTime);

        // 🎥 volta pro jogador
        Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y, cameraSegue.cameraZ);
        yield return StartCoroutine(MoveCamera(cameraTransform.position, playerPos));

        // 🔁 reativa o follow
        cameraSegue.player = originalTarget;

        // 🔓 libera o player
        if (playerController != null)
            playerController.canMove = true;

        // ⚔️ ativa o inimigo
        if (enemyToActivate != null)
            enemyToActivate.SetActive(true);

        Debug.Log("[DiningRoomCutscene] Cutscene concluída. Combate iniciado!");
    }

    private IEnumerator MoveCamera(Vector3 start, Vector3 end)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            cameraTransform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}
