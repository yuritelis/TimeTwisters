using UnityEngine;
using System.Collections;

public class BossEdward_Leap_Attack : MonoBehaviour
{
    [Header("Configuração do Salto")]
    public float leapDuration = 0.8f;
    public float leapSpeed = 8f;
    public int damage = 2;
    public LayerMask playerLayer;
    public Color leapColor = Color.red;

    [Header("Telegraph Visual")]
    public GameObject telegraphPrefab;
    public float telegraphWidth = 1f;
    public float telegraphTime = 1f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isLeaping = false;
    private GameObject activeTelegraph;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator DoLeap(Transform player, BossEdwardController boss)
    {
        if (isLeaping || player == null || boss == null || boss.isDead)
            yield break;

        isLeaping = true;

        Color originalColor = sr.color;
        Vector2 startPos = transform.position;
        float maxDistance = leapSpeed * leapDuration;

        GameObject telegraph = null;
        if (telegraphPrefab != null)
        {
            telegraph = Instantiate(telegraphPrefab, startPos, Quaternion.identity);
            telegraph.transform.localScale = new Vector3(0f, telegraphWidth, 1f);

            // 🔥 Transparência aplicada aqui
            var srTele = telegraph.GetComponent<SpriteRenderer>();
            if (srTele != null)
            {
                Color c = srTele.color;
                c.a = 0.35f;
                srTele.color = c;
            }

            activeTelegraph = telegraph;
        }

        float elapsed = 0f;
        Vector2 finalTargetPos = startPos;

        while (elapsed < telegraphTime)
        {
            if (boss == null || boss.isDead)
            {
                if (telegraph != null) Destroy(telegraph);
                sr.color = originalColor;
                isLeaping = false;
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / telegraphTime);

            finalTargetPos = player.position;
            Vector2 leapDirection = (finalTargetPos - startPos).normalized;
            float distanceToPlayer = Vector2.Distance(startPos, finalTargetPos);
            float currentLength = Mathf.Min(Mathf.Lerp(0f, distanceToPlayer, t * t), maxDistance);

            if (telegraph != null)
            {
                telegraph.transform.localScale = new Vector3(currentLength, telegraphWidth, 1f);
                telegraph.transform.rotation = Quaternion.FromToRotation(Vector3.right, leapDirection);
                telegraph.transform.position = startPos;
            }

            sr.color = leapColor;
            yield return null;
        }

        sr.color = originalColor;
        if (telegraph != null)
            Destroy(telegraph);

        Vector2 leapDir = (finalTargetPos - startPos).normalized;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), true);

        var originalBodyType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;

        elapsed = 0f;
        while (elapsed < leapDuration)
        {
            if (boss == null || boss.isDead)
            {
                sr.color = originalColor;
                rb.bodyType = originalBodyType;
                isLeaping = false;
                yield break;
            }

            elapsed += Time.deltaTime;
            transform.position += (Vector3)(leapDir * leapSpeed * Time.deltaTime);

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

        rb.bodyType = originalBodyType;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), false);
        isLeaping = false;
    }

    public void CleanupAfterDeath()
    {
        if (activeTelegraph != null)
        {
            Destroy(activeTelegraph);
            activeTelegraph = null;
        }
    }
}
