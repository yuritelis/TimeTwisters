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

        // 🔹 Calcula posição desejada da câmera (seguindo o player com offset)
        Vector3 posToGo = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            cameraZ
        );

        // 🔒 Aplica limites de câmera, se houver collider de mapa
        if (mapaCollider != null)
        {
            float clampX = Mathf.Clamp(posToGo.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
            float clampY = Mathf.Clamp(posToGo.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
            posToGo = new Vector3(clampX, clampY, posToGo.z);
        }

        // 🔁 Movimento suave
        transform.position = Vector3.Lerp(transform.position, posToGo, velocidade);
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
}
