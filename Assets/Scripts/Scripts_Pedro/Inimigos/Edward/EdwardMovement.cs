using UnityEngine;

public class EdwardMovement : MonoBehaviour
{
    [Header("Movimento e Ataque")]
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float playerDetectRange = 5f;
    public float attackCooldown = 2f;

    [Header("Trava de Ataque")]
    public float attackLockTime = 0.4f;
    private float attackLockTimer = 0f;

    [Header("Referências")]
    public Transform attackPoint;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer = 0f;
    private Rigidbody2D rb;
    private Animator anim;
    public Transform player;
    private EdwardState enemyState;
    private bool canAttack = true;
    private int facingDirection = -1;

    [HideInInspector]
    public bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EdwardState.Idle);
    }

    private void Update()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0) canAttack = true;
        }

        if (attackLockTimer > 0)
            attackLockTimer -= Time.deltaTime;

        CheckForPlayer();

        switch (enemyState)
        {
            case EdwardState.Chasing:
                if (player != null)
                {
                    float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);
                    if (distanceToPlayer > attackRange) Chase();
                    else rb.linearVelocity = Vector2.zero;
                }
                break;

            case EdwardState.Attacking:
                rb.linearVelocity = Vector2.zero;
                if (player != null)
                {
                    float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);
                    if (distanceToPlayer > attackRange && attackLockTimer <= 0)
                        ChangeState(EdwardState.Chasing);
                }
                break;

            case EdwardState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                if (canAttack && enemyState != EdwardState.Attacking)
                {
                    ChangeState(EdwardState.Attacking);
                    canAttack = false;
                    attackCooldownTimer = attackCooldown;
                    attackLockTimer = attackLockTime;
                }
            }
            else if (enemyState != EdwardState.Chasing)
                ChangeState(EdwardState.Chasing);
        }
        else if (enemyState != EdwardState.Idle)
        {
            ChangeState(EdwardState.Idle);
            player = null;
        }
    }

    private void Chase()
    {
        if (player == null) return;

        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            //Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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
            float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                ChangeState(EdwardState.Idle);
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        ChangeState(EdwardState.Chasing);
    }

    public void LockAttack()
    {
        attackLockTimer = attackLockTime;
    }

    /// <summary>
    /// Força um ataque normal pelo BossEdwardController
    /// </summary>
    public void ForceNormalAttack()
    {
        if (canAttack && player != null)
        {
            Debug.Log("ForceNormalAttack: Iniciando ataque");
            ChangeState(EdwardState.Attacking);
            canAttack = false;
            attackCooldownTimer = attackCooldown;
            attackLockTimer = attackLockTime;

            // EXECUTA O ATAQUE IMEDIATAMENTE - ISSO QUE TAVA FALTANDO
            Attack();
        }
        else
        {
            Debug.Log($"ForceNormalAttack: Não pode atacar - canAttack: {canAttack}, player: {player != null}");
        }
    }

    private void ChangeState(EdwardState newState)
    {
        enemyState = newState;

        if (anim != null && anim.runtimeAnimatorController != null)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isChasing", false);
            anim.SetBool("isAttacking", false);

            switch (enemyState)
            {
                case EdwardState.Idle: anim.SetBool("isIdle", true); break;
                case EdwardState.Chasing: anim.SetBool("isChasing", true); break;
                case EdwardState.Attacking: anim.SetBool("isAttacking", true); break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}

public enum EdwardState
{
    Idle,
    Chasing,
    Attacking
}