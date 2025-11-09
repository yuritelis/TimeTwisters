using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSegue : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Collider2D mapaCollider; // Collider que define os limites do mapa

    [Header("Movimento")]
    [Range(0f, 1f)] public float velocidade = 0.05f;

    [Header("Configuração")]
    public float cameraZ = -10f; // 🔹 garante que o Z nunca muda
    public Vector2 offset;       // opcional: leve deslocamento da câmera em relação ao player

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    // ===================== 🔹 NOVOS CAMPOS PARA CUTSCENE 🔹 =====================
    private bool overridingTarget = false;    // indica se há alvo temporário
    private Transform overrideTarget;         // alvo alternativo da câmera
    [Range(0f, 1f)] public float velocidadeCutscene = 0.1f; // velocidade separada para cutscenes
    // ===========================================================================

    void Start()
    {
        cam = GetComponent<Camera>();

        // 🔹 Procura o player automaticamente se não estiver atribuído
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                player = found.transform;
        }

        // 🔹 Garante que a câmera já começa exatamente sobre o player
        if (player != null)
        {
            Vector3 startPos = new Vector3(player.position.x, player.position.y, cameraZ);
            transform.position = startPos;
        }

        // 🔹 Calcula o tamanho da câmera em unidades do mundo
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        // 🔹 Se o collider do mapa foi definido, pega os limites dele
        if (mapaCollider != null)
        {
            Bounds b = mapaCollider.bounds;
            minBounds = b.min;
            maxBounds = b.max;
        }
        else
        {
            Debug.LogWarning("[CameraSegue] Nenhum mapaCollider definido — sem limites de câmera.");
        }

        // 🔹 Garante Z correto
        Vector3 pos = transform.position;
        pos.z = cameraZ;
        transform.position = pos;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // ===================== 🔹 ALTERAÇÃO AQUI 🔹 =====================
        // agora a câmera segue o player normalmente, mas se houver "override",
        // ela segue o alvo temporário (ex: durante uma cutscene)
        Transform target = overridingTarget && overrideTarget != null ? overrideTarget : player;
        // ===============================================================

        // 🔹 Calcula posição desejada da câmera (seguindo o alvo com offset)
        Vector3 posToGo = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            cameraZ
        );

        // 🔒 Aplica limites de câmera, se houver collider de mapa
        if (mapaCollider != null)
        {
            float clampX = Mathf.Clamp(posToGo.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
            float clampY = Mathf.Clamp(posToGo.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
            posToGo = new Vector3(clampX, clampY, posToGo.z);
        }

        // 🔁 Movimento suave — usa velocidade normal ou da cutscene
        float vel = overridingTarget ? velocidadeCutscene : velocidade;
        transform.position = Vector3.Lerp(transform.position, posToGo, vel);
    }

    void OnDrawGizmosSelected()
    {
        if (mapaCollider != null)
        {
            Gizmos.color = Color.yellow;
            Bounds b = mapaCollider.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }

    // ===================== 🔹 NOVAS FUNÇÕES DE CUTSCENE 🔹 =====================

    public void BeginTemporaryFocus(Transform target)
    {
        overrideTarget = target;
        overridingTarget = true;
    }

    public void EndTemporaryFocus()
    {
        overridingTarget = false;
        overrideTarget = null;
    }

    public System.Collections.IEnumerator FocusSequence(Transform target, float settleTime, float holdTime)
    {
        BeginTemporaryFocus(target);
        yield return new WaitForSeconds(settleTime);
        yield return new WaitForSeconds(holdTime);
        EndTemporaryFocus();
    }
    // ===========================================================================
}
