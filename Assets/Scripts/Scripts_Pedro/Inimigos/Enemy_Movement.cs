using System.Xml.Linq;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Movimento e Ataque")]
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float playerDetectRange = 5f;
    public float attackCooldown = 2f;

    [Header("Trava de Ataque")]
    public float attackLockTime = 0.4f;
    private float attackLockTimer = 0f;

    [Header("Patrulha")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool isReversing = false; // ping-pong
    public float patrolWaitTime = 2f;
    private float patrolWaitTimer = 0f;

    [Header("Referências")]
    public Transform attackPoint;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("Campo de Visão")]
    [Range(0f, 180f)]
    public float visionAngle = 45f;          // meia-abertura do cone
    public LayerMask visionBlockMask;        // paredes/chão que bloqueiam visão (NÃO inclua Player)

    private float attackCooldownTimer = 0f;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    public Transform player;

    private EnemyState enemyState;
    private bool canAttack = true;

    public Vector2 facingDirection = Vector2.left;

    private float initialAttackDist;
    private float initialDetectDist;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        initialAttackDist = Vector2.Distance(transform.position, attackPoint.position);
        initialDetectDist = Vector2.Distance(transform.position, detectionPoint.position);

        ApplyPointRotation();
        ChangeState(EnemyState.Patrolling);
    }

    private void Update()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
                canAttack = true;
        }

        if (attackLockTimer > 0)
            attackLockTimer -= Time.deltaTime;

        CheckForPlayer();

        switch (enemyState)
        {
            case EnemyState.Chasing:
                if (player != null)
                {
                    float distance = Vector2.Distance(attackPoint.position, player.position);
                    if (distance > attackRange)
                        Chase();
                    else
                        rb.linearVelocity = Vector2.zero;
                }
                break;

            case EnemyState.Attacking:
                rb.linearVelocity = Vector2.zero;
                if (player != null)
                {
                    float distance = Vector2.Distance(attackPoint.position, player.position);
                    if (distance > attackRange && attackLockTimer <= 0)
                        ChangeState(EnemyState.Chasing);
                }
                break;

            case EnemyState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;

            case EnemyState.Patrolling:
                Patrol();
                break;
        }
    }

    // =======================
    // DETECÇÃO (cone + LOS)
    // =======================
    private void CheckForPlayer()
    {
        // Pegamos quem está no raio máximo, depois filtramos por cone e LOS
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        bool playerDetected = false;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            // Direção e distância até o player
            Vector2 dirToPlayer = ((Vector2)hit.transform.position - (Vector2)detectionPoint.position).normalized;
            float distToPlayer = Vector2.Distance(detectionPoint.position, hit.transform.position);

            // 1) Checagem do cone (ângulo)
            float angle = Vector2.Angle(facingDirection, dirToPlayer);
            if (angle > visionAngle) continue;

            // 2) Linha de visão: se houver QUALQUER bloqueio entre inimigo e player, não vê
            RaycastHit2D blocker = Physics2D.Raycast(detectionPoint.position, dirToPlayer, distToPlayer, visionBlockMask);
            if (blocker.collider != null)
            {
                // Tem parede no meio -> não detecta
                continue;
            }

            // Passou: dentro do cone e sem bloqueio
            player = hit.transform;
            playerDetected = true;
            break;
        }

        if (playerDetected)
        {
            float distance = Vector2.Distance(attackPoint.position, player.position);

            if (distance <= attackRange && canAttack && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Attacking);
                canAttack = false;
                attackCooldownTimer = attackCooldown;
                attackLockTimer = attackLockTime;
            }
            else if (distance > attackRange && enemyState != EnemyState.Chasing)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            if (enemyState != EnemyState.Patrolling)
            {
                ChangeState(EnemyState.Patrolling);
                player = null;
            }
        }
    }

    private void Chase()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        UpdateFacingDirection(direction);

        rb.linearVelocity = direction * speed;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        UpdateFacingDirection(direction);
        rb.linearVelocity = direction * speed;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance < 0.2f)
        {
            rb.linearVelocity = Vector2.zero;

            if (patrolWaitTimer <= 0f)
            {
                patrolWaitTimer = patrolWaitTime;

                // ping-pong
                if (!isReversing)
                {
                    if (currentPatrolIndex >= patrolPoints.Length - 1)
                    {
                        isReversing = true;
                        currentPatrolIndex--;
                    }
                    else
                    {
                        currentPatrolIndex++;
                    }
                }
                else
                {
                    if (currentPatrolIndex <= 0)
                    {
                        isReversing = false;
                        currentPatrolIndex++;
                    }
                    else
                    {
                        currentPatrolIndex--;
                    }
                }
            }
            else
            {
                patrolWaitTimer -= Time.deltaTime;
            }
        }
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            facingDirection = direction.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            facingDirection = direction.y > 0 ? Vector2.up : Vector2.down;
        }

        ApplyPointRotation();
    }

    private void ApplyPointRotation()
    {
        if (attackPoint == null || detectionPoint == null)
            return;

        float angle = 0f;
        Vector2 offset = Vector2.zero;

        if (facingDirection == Vector2.up)
        {
            angle = 90f;
            offset = Vector2.up;
        }
        else if (facingDirection == Vector2.down)
        {
            angle = -90f;
            offset = Vector2.down;
        }
        else if (facingDirection == Vector2.left)
        {
            angle = 180f;
            offset = Vector2.left;
        }
        else if (facingDirection == Vector2.right)
        {
            angle = 0f;
            offset = Vector2.right;
        }

        attackPoint.localRotation = Quaternion.Euler(0, 0, angle);
        detectionPoint.localRotation = Quaternion.Euler(0, 0, angle);

        attackPoint.localPosition = offset * initialAttackDist;
        detectionPoint.localPosition = offset * initialDetectDist;

        if (sr != null)
            sr.flipX = (facingDirection == Vector2.left);

        attackPoint.localScale = Vector3.one;
        detectionPoint.localScale = Vector3.one;
    }

    public void Attack()
    {
        GetComponent<EnemyCombat>()?.Attack();
    }

    public void EndAttack()
    {
        attackCooldownTimer = attackCooldown;
        canAttack = false;

        if (player != null)
        {
            float distance = Vector2.Distance(attackPoint.position, player.position);
            if (distance <= attackRange)
            {
                ChangeState(EnemyState.Idle);
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        ChangeState(EnemyState.Chasing);
    }

    public void LockAttack()
    {
        attackLockTimer = attackLockTime;
    }

    private void ChangeState(EnemyState newState)
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isChasing", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isPatrolling", false);

        enemyState = newState;

        switch (enemyState)
        {
            case EnemyState.Idle: anim.SetBool("isIdle", true); break;
            case EnemyState.Chasing: anim.SetBool("isChasing", true); break;
            case EnemyState.Attacking: anim.SetBool("isAttacking", true); break;
            case EnemyState.Patrolling: anim.SetBool("isPatrolling", true); break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // CONE DE VISÃO (sem círculo)
        if (detectionPoint != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.9f);

            // base do cone = facingDirection a partir do detectionPoint
            Vector2 forward = facingDirection == Vector2.zero ? Vector2.right : facingDirection;

            // desenha as duas bordas do cone
            Vector3 leftDir = Quaternion.Euler(0, 0, visionAngle) * (Vector3)forward;
            Vector3 rightDir = Quaternion.Euler(0, 0, -visionAngle) * (Vector3)forward;
            Gizmos.DrawLine(detectionPoint.position, detectionPoint.position + leftDir * playerDetectRange);
            Gizmos.DrawLine(detectionPoint.position, detectionPoint.position + rightDir * playerDetectRange);

            // desenha o arco do cone (apenas linhas, sem círculo)
            int steps = 24;
            float start = -visionAngle;
            float stepAngle = (visionAngle * 2f) / steps;
            Vector3 prev = detectionPoint.position + (Quaternion.Euler(0, 0, start) * (Vector3)forward) * playerDetectRange;
            for (int i = 1; i <= steps; i++)
            {
                float a = start + stepAngle * i;
                Vector3 next = detectionPoint.position + (Quaternion.Euler(0, 0, a) * (Vector3)forward) * playerDetectRange;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (patrolPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.2f);
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)facingDirection * 1f);
    }

    public void ReceiveStealthKill()
    {
        Debug.Log($"☠️ {name} eliminado por stealth.");

        // aqui você pode tocar uma animação de morte se quiser:
        // anim.SetTrigger("Die");

        // opcional: desativar colisões
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // remove o inimigo após 0.5s
        Destroy(gameObject, 0.5f);
    }
}


public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Patrolling
}
