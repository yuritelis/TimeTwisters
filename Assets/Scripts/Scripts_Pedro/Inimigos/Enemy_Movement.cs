using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Movimento e Ataque")]
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float playerDetectRange = 5f;
    public float attackCooldown = 2f;

    [Header("Trava de Ataque")]
    [Tooltip("Tempo durante o qual o inimigo não pode cancelar o ataque, mesmo que o jogador saia do alcance.")]
    public float attackLockTime = 0.4f;
    private float attackLockTimer = 0f;

    [Header("Patrulha")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    public float patrolWaitTime = 2f;
    private float patrolWaitTimer = 0f;

    [Header("Referências")]
    public Transform attackPoint;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer = 0f;
    private Rigidbody2D rb;
    private Animator anim;
    public Transform player;

    private EnemyState enemyState;
    private bool canAttack = true;
    public int facingDirection = -1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Patrolling); // Começa patrulhando
    }

    private void Update()
    {
        // Atualiza cooldown de ataque
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
                canAttack = true;
        }

        // Atualiza o timer da trava de ataque
        if (attackLockTimer > 0)
        {
            attackLockTimer -= Time.deltaTime;
        }

        // Verifica se o jogador está dentro da área de detecção
        CheckForPlayer();

        // Comportamento baseado no estado atual
        switch (enemyState)
        {
            case EnemyState.Chasing:
                if (player != null)
                {
                    float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);
                    if (distanceToPlayer > attackRange)
                        Chase();
                    else
                        rb.linearVelocity = Vector2.zero; // se estiver perto demais, para
                }
                break;

            case EnemyState.Attacking:
                rb.linearVelocity = Vector2.zero;

                if (player != null)
                {
                    float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);

                    // Só pode sair do ataque depois que o tempo de lock acabar
                    if (distanceToPlayer > attackRange && attackLockTimer <= 0)
                    {
                        ChangeState(EnemyState.Chasing);
                    }
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

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                if (canAttack && enemyState != EnemyState.Attacking)
                {
                    ChangeState(EnemyState.Attacking);
                    canAttack = false;
                    attackCooldownTimer = attackCooldown;
                    attackLockTimer = attackLockTime;
                }
            }
            else if (enemyState != EnemyState.Chasing)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else if (enemyState != EnemyState.Patrolling)
        {
            ChangeState(EnemyState.Patrolling);
            player = null;
        }
    }

    private void Chase()
    {
        if (player == null) return;

        // Vira na direção do jogador
        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            //Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        float distance = Vector2.Distance(transform.position, targetPoint.position);
        if (distance < 0.2f)
        {
            rb.linearVelocity = Vector2.zero;

            if (patrolWaitTimer <= 0f)
            {
                patrolWaitTimer = patrolWaitTime;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
            else
            {
                patrolWaitTimer -= Time.deltaTime;
            }
        }

        // Vira o inimigo na direção do ponto
        if ((targetPoint.position.x > transform.position.x && facingDirection == -1) ||
            (targetPoint.position.x < transform.position.x && facingDirection == 1))
        {
            // Flip();
        }
    }

    private void Flip()
    {
        facingDirection *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // 🔁 Inverter posição do detectionPoint
        if (detectionPoint != null)
        {
            Vector3 pos = detectionPoint.localPosition;
            pos.x *= -1;
            detectionPoint.localPosition = pos;
        }

        // 🔁 Também inverta attackPoint, se necessário
        if (attackPoint != null)
        {
            Vector3 pos = attackPoint.localPosition;
            pos.x *= -1;
            attackPoint.localPosition = pos;
        }
    }


    // Chamado pelo evento de animação durante o ataque
    public void Attack()
    {
        GetComponent<EnemyCombat>()?.Attack();
    }

    // Chamado pelo último evento da animação de ataque
    public void EndAttack()
    {
        attackCooldownTimer = attackCooldown;
        canAttack = false;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);
            if (distanceToPlayer <= attackRange)
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
            case EnemyState.Idle:
                anim.SetBool("isIdle", true);
                break;
            case EnemyState.Chasing:
                anim.SetBool("isChasing", true);
                break;
            case EnemyState.Attacking:
                anim.SetBool("isAttacking", true);
                break;
            case EnemyState.Patrolling:
                anim.SetBool("isPatrolling", true);
                break;
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

        // Desenhar pontos de patrulha
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.2f);
            }
        }
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Patrolling
}
