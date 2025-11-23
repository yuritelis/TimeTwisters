using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class BossEdward_Leap_Attack : MonoBehaviour
{
    public float leapDuration = 0.8f;
    public float leapSpeed = 8f;
    public int damage = 2;
    public LayerMask playerLayer;
    public Color leapColor = Color.red;
    public GameObject telegraphPrefab;
    public float telegraphWidth = 1f;
    public float telegraphTime = 1f;
    public GameObject healingItemPrefab;
    public float itemFallGravity = 3f;
    public Camera playerCamera;
    public Tilemap groundTilemap;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isLeaping = false;
    private GameObject activeTelegraph;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = playerCamera.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            playerCamera.transform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.transform.localPosition = originalPos;
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
            BossEdwardCleanup.Register(telegraph);
            telegraph.transform.localScale = new Vector3(0f, telegraphWidth, 1f);
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

            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(telegraphWidth, telegraphWidth), 0f, playerLayer);
            foreach (var hit in hits)
            {
                hit.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
            }

            Collider2D wallHit = Physics2D.OverlapCircle(transform.position, 0.25f, LayerMask.GetMask("Collision"));
            if (wallHit != null)
            {
                boss.bossHp.ChangeHealth(-1);
                StartCoroutine(CameraShake(0.15f, 0.2f));
                SpawnHealingItemInsideCameraOnGround();
                break;
            }

            yield return null;
        }

        rb.bodyType = originalBodyType;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), false);
        isLeaping = false;
    }

    void SpawnHealingItemInsideCameraOnGround()
    {
        if (healingItemPrefab == null) return;

        for (int i = 0; i < 40; i++)
        {
            Vector3 viewport = new Vector3(Random.value, Random.value, 0f);
            Vector3 worldPos = playerCamera.ViewportToWorldPoint(viewport);
            worldPos.z = 0;

            Vector3Int cell = groundTilemap.WorldToCell(worldPos);
            if (groundTilemap.HasTile(cell))
            {
                Vector3 dropPos = groundTilemap.GetCellCenterWorld(cell);
                InstantiateHealingItem(dropPos);
                return;
            }
        }

        InstantiateHealingItem(transform.position);
    }

    void InstantiateHealingItem(Vector3 pos)
    {
        GameObject item = Instantiate(healingItemPrefab, pos, Quaternion.identity);
        Rigidbody2D rb = item.AddComponent<Rigidbody2D>();
        rb.gravityScale = itemFallGravity;
        var col = item.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        item.AddComponent<HealingItemFall>();
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

public class HealingItemFall : MonoBehaviour
{
    public float fallTime = 0.45f;
    public float startHeightOffset = 3f;

    float elapsed = 0f;
    Vector3 startPos;
    Vector3 finalPos;

    SpriteRenderer sr;
    Collider2D col;
    Rigidbody2D rb;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (col != null) col.enabled = false;

        finalPos = transform.position;
        startPos = finalPos + new Vector3(0f, startHeightOffset, 0f);

        transform.position = startPos;

        if (sr != null)
            sr.sortingOrder = 500;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / fallTime);

        transform.position = Vector3.Lerp(startPos, finalPos, t);

        if (sr != null)
            sr.sortingOrder = Mathf.RoundToInt(Mathf.Lerp(500, 10, t));

        if (t >= 1f)
        {
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
            }

            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            Destroy(this);
        }
    }
}
