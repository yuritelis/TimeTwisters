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
    private bool isReversing = false;
    public float patrolWaitTime = 2f;
    private float patrolWaitTimer = 0f;

    [Header("Referências")]
    public Transform attackPoint;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("Campo de Visão")]
    [Range(0f, 180f)] public float visionAngle = 45f;
    public float chaseVisionRadius = 6f;
    public LayerMask visionBlockMask;

    [Header("Desvio de Obstáculos (sem NavMesh)")]
    public LayerMask obstacleMask;
    public float collisionCheckDistance = 0.55f;

    private float attackCooldownTimer = 0f;
    private Rigidbody2D rb;
    private Animator anim;
    public Transform player;

    private EnemyState enemyState;
    private bool canAttack = true;

    public Vector2 facingDirection = Vector2.left;
    private Vector2 lastValidDirection = Vector2.left;

    private Vector2 smoothDirection;
    private Vector2 desiredDirection;

    private float stuckTimer = 0f;
    private const float stuckThreshold = 0.25f;

    private float initialAttackDist;
    private float initialDetectDist;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (attackPoint != null)
            initialAttackDist = Vector2.Distance(transform.position, attackPoint.position);
        if (detectionPoint != null)
            initialDetectDist = Vector2.Distance(transform.position, detectionPoint.position);

        ApplyPointRotation();
        ChangeState(EnemyState.Patrolling);
    }

    private void Update()
    {
        HandleAnimation();

        // Timers
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
                    float dist = Vector2.Distance(attackPoint.position, player.position);

                    if (dist > attackRange)
                        Chase();
                    else
                        rb.linearVelocity = Vector2.zero;
                }
                break;

            case EnemyState.Attacking:
                rb.linearVelocity = Vector2.zero;

                if (player != null)
                {
                    float dist2 = Vector2.Distance(attackPoint.position, player.position);
                    if (dist2 > attackRange && attackLockTimer <= 0)
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

    private void HandleAnimation()
    {
        Vector2 vel = rb.linearVelocity;
        float minMove = 0.08f;

        if (vel.sqrMagnitude > minMove * minMove)
        {
            anim.SetBool("isMoving", true);

            Vector2 dir = vel.normalized;
            anim.SetFloat("moveX", dir.x);
            anim.SetFloat("moveY", dir.y);

            lastValidDirection = dir;
            UpdateFacingFromLastDirection();
        }
        else
        {
            anim.SetBool("isMoving", false);
            anim.SetFloat("moveX", lastValidDirection.x);
            anim.SetFloat("moveY", lastValidDirection.y);
        }
    }

    private void UpdateFacingFromLastDirection()
    {
        if (lastValidDirection.sqrMagnitude < 0.001f)
            return;

        if (Mathf.Abs(lastValidDirection.x) > Mathf.Abs(lastValidDirection.y))
            facingDirection = lastValidDirection.x > 0 ? Vector2.right : Vector2.left;
        else
            facingDirection = lastValidDirection.y > 0 ? Vector2.up : Vector2.down;

        ApplyPointRotation();
    }

    private Vector2 GetSmoothedDirection(Vector2 desired)
    {
        if (desired.sqrMagnitude < 0.01f)
            return Vector2.zero;

        desired = desired.normalized;

        if (!Physics2D.Raycast(transform.position, desired, collisionCheckDistance, obstacleMask))
            return desired;

        Vector2[] lvl1 =
        {
            (Vector2)(Quaternion.Euler(0, 0, 40f) * desired),
            (Vector2)(Quaternion.Euler(0, 0, -40f) * desired),
            new Vector2(desired.y, -desired.x),
            new Vector2(-desired.y, desired.x)
        };

        foreach (var dir in lvl1)
            if (!Physics2D.Raycast(transform.position, dir, collisionCheckDistance, obstacleMask))
                return dir.normalized;

        foreach (var prev in lvl1)
        {
            Vector2[] lvl2 =
            {
                (Vector2)(Quaternion.Euler(0, 0, 25f) * prev),
                (Vector2)(Quaternion.Euler(0, 0, -25f) * prev)
            };

            foreach (var dir in lvl2)
                if (!Physics2D.Raycast(transform.position, dir, collisionCheckDistance, obstacleMask))
                    return dir.normalized;
        }

        return -desired;
    }

    private void MoveTowards(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        desiredDirection = direction.normalized;
        smoothDirection = GetSmoothedDirection(desiredDirection);

        Vector2 finalDir = Vector2.Lerp(rb.linearVelocity.normalized, smoothDirection, 10f * Time.deltaTime);

        rb.linearVelocity = finalDir * speed;

        if (rb.linearVelocity.magnitude < speed * 0.4f)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer >= stuckThreshold)
        {
            Vector2 perp1 = new Vector2(finalDir.y, -finalDir.x);
            Vector2 perp2 = new Vector2(-finalDir.y, finalDir.x);

            if (!Physics2D.Raycast(transform.position, perp1, collisionCheckDistance, obstacleMask))
                rb.linearVelocity = perp1 * speed;
            else if (!Physics2D.Raycast(transform.position, perp2, collisionCheckDistance, obstacleMask))
                rb.linearVelocity = perp2 * speed;
            else
                rb.linearVelocity = -finalDir * speed;

            stuckTimer = 0f;
        }
    }

    private void Chase()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        MoveTowards(dir);
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];
        Vector2 dir = (target.position - transform.position).normalized;

        MoveTowards(dir);

        float dist = Vector2.Distance(transform.position, target.position);

        if (dist < 0.25f)
        {
            rb.linearVelocity = Vector2.zero;

            if (patrolWaitTimer <= 0f)
            {
                patrolWaitTimer = patrolWaitTime;

                if (!isReversing)
                {
                    if (currentPatrolIndex >= patrolPoints.Length - 1)
                    {
                        isReversing = true;
                        currentPatrolIndex--;
                    }
                    else currentPatrolIndex++;
                }
                else
                {
                    if (currentPatrolIndex <= 0)
                    {
                        isReversing = false;
                        currentPatrolIndex++;
                    }
                    else currentPatrolIndex--;
                }
            }
            else patrolWaitTimer -= Time.deltaTime;
        }
    }

    private void CheckForPlayer()
    {
        bool found = false;

        float detectRange = enemyState == EnemyState.Chasing ? chaseVisionRadius : playerDetectRange;
        float currentAngle = enemyState == EnemyState.Chasing ? 180f : visionAngle;

        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, detectRange, playerLayer);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            Vector2 dirToPlayer = ((Vector2)hit.transform.position - (Vector2)detectionPoint.position).normalized;
            float dist = Vector2.Distance(detectionPoint.position, hit.transform.position);

            if (enemyState != EnemyState.Chasing)
            {
                float angle = Vector2.Angle(facingDirection, dirToPlayer);
                if (angle > currentAngle) continue;

                RaycastHit2D block = Physics2D.Raycast(
                    detectionPoint.position,
                    dirToPlayer,
                    dist,
                    visionBlockMask);

                if (block.collider != null)
                    continue;
            }

            found = true;
            player = hit.transform;
            break;
        }

        if (found)
        {
            float dist = Vector2.Distance(attackPoint.position, player.position);

            if (dist <= attackRange && canAttack && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Attacking);
                canAttack = false;
                attackCooldownTimer = attackCooldown;
                attackLockTimer = attackLockTime;
            }
            else if (dist > attackRange && enemyState != EnemyState.Chasing)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            if (enemyState == EnemyState.Chasing)
            {
                ChangeState(EnemyState.Patrolling);

                float closest = Mathf.Infinity;
                int idx = 0;

                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    float d = Vector2.Distance(transform.position, patrolPoints[i].position);
                    if (d < closest)
                    {
                        closest = d;
                        idx = i;
                    }
                }

                currentPatrolIndex = idx;
                player = null;
            }
        }
    }

    public void Attack()
    {
        GetComponent<EnemyCombat>()?.Attack();
    }

    public void EndAttack()
    {
        anim.SetBool("isAttacking", false);

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

    public void ReceiveStealthKill()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        this.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        var anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Die");

        var knock = GetComponent<Enemy_Knockback>();
        if (knock != null)
            knock.enabled = false;

        Destroy(gameObject, 0.5f);
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
<<<<<<< HEAD

        //if (sr != null)
        //    sr.flipX = (facingDirection == Vector2.left);
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
=======
>>>>>>> origin/main
    }

    private void ChangeState(EnemyState newState)
    {
        enemyState = newState;

        if (newState == EnemyState.Attacking)
        {
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("Attack");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            // Vision range normal
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.35f);
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
            Gizmos.DrawWireSphere(detectionPoint.position, chaseVisionRadius);

            Gizmos.color = Color.red;
            Vector2 forward = facingDirection == Vector2.zero ? Vector2.right : facingDirection;

            Vector3 leftDir = Quaternion.Euler(0, 0, visionAngle) * (Vector3)forward;
            Vector3 rightDir = Quaternion.Euler(0, 0, -visionAngle) * (Vector3)forward;

            Gizmos.DrawLine(detectionPoint.position, detectionPoint.position + leftDir * playerDetectRange);
            Gizmos.DrawLine(detectionPoint.position, detectionPoint.position + rightDir * playerDetectRange);

            int steps = 24;
            float start = -visionAngle;
            float stepAngle = (visionAngle * 2f) / steps;

            Vector3 prev = detectionPoint.position +
                           (Quaternion.Euler(0, 0, start) * (Vector3)forward) * playerDetectRange;

            for (int i = 1; i <= steps; i++)
            {
                float a = start + stepAngle * i;
                Vector3 next = detectionPoint.position +
                               (Quaternion.Euler(0, 0, a) * (Vector3)forward) * playerDetectRange;

                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, collisionCheckDistance);

        if (patrolPoints != null)
        {
            Gizmos.color = Color.magenta;
            foreach (Transform p in patrolPoints)
            {
                if (p != null)
                    Gizmos.DrawSphere(p.position, 0.15f);
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)facingDirection);
    }

}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Patrolling
}
