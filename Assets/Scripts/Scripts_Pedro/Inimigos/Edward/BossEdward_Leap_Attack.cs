using UnityEngine;
using System.Collections;

public class BossEdward_Leap_Attack : MonoBehaviour
{
    public float leapDuration = 0.8f;
    public float leapSpeed = 8f;
    public int damage = 2;
    public LayerMask playerLayer;
    public Color leapColor = Color.red;

    [Header("Telegraph visual")]
    public GameObject telegraphPrefab;
    public float telegraphWidth = 1f;
    public float telegraphTime = 1f; // tempo de carregamento do telegraph

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isLeaping = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator DoLeap(Transform player)
    {
        if (isLeaping || player == null) yield break;
        isLeaping = true;

        Color originalColor = sr.color;
        Vector2 startPos = transform.position;

        // Distância máxima que o boss vai percorrer
        float maxDistance = leapSpeed * leapDuration;

        // Instancia telegraph
        GameObject telegraph = null;
        if (telegraphPrefab != null)
        {
            telegraph = Instantiate(telegraphPrefab, startPos, Quaternion.identity);
            telegraph.transform.localScale = new Vector3(0f, telegraphWidth, 1f);
        }

        // Carregamento do telegraph (segue o player, mas limitado a maxDistance)
        float elapsed = 0f;
        Vector2 finalTargetPos = startPos; // posição final do player
        while (elapsed < telegraphTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / telegraphTime);

            finalTargetPos = player.position;
            Vector2 leapDirection = (finalTargetPos - startPos).normalized;
            float distanceToPlayer = Vector2.Distance(startPos, finalTargetPos);

            // Crescimento exponencial, mas limitado à distância máxima
            float currentLength = Mathf.Min(Mathf.Lerp(0f, distanceToPlayer, t * t), maxDistance);

            if (telegraph != null)
            {
                telegraph.transform.localScale = new Vector3(currentLength, telegraphWidth, 1f);
                telegraph.transform.rotation = Quaternion.FromToRotation(Vector3.right, leapDirection);
                telegraph.transform.position = startPos;
            }

            // Cor vermelha só durante o carregamento
            sr.color = leapColor;

            yield return null;
        }

        // Restaurar cor antes do salto
        sr.color = originalColor;

        if (telegraph != null)
            Destroy(telegraph);

        // === INÍCIO DO SALTO ===
        Vector2 leapDir = (finalTargetPos - startPos).normalized;

        // ⛔ Ignora colisão física com o player
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), true);

        var originalBodyType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;

        elapsed = 0f;
        while (elapsed < leapDuration)
        {
            elapsed += Time.deltaTime;

            // Move o boss
            Vector3 move = leapDir * leapSpeed * Time.deltaTime;
            transform.position += move;

            // Dano na posição atual do boss
            Vector2 boxSize = new Vector2(telegraphWidth, telegraphWidth);
            Vector2 boxCenter = transform.position;
            float angle = Mathf.Atan2(leapDir.y, leapDir.x) * Mathf.Rad2Deg;

            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, angle, playerLayer);
            foreach (var hit in hits)
            {
                hit.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
            }

            yield return null;
        }

        // === FIM DO SALTO ===
        rb.bodyType = originalBodyType;

        // ✅ Reativa colisão com o player
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), false);

        isLeaping = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(telegraphWidth, telegraphWidth, 1f));
    }
}
