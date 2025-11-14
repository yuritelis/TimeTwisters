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

    [HideInInspector] public bool canMove = true;

    private bool lastIsAttackingState = false;
    public EdwardState CurrentState => enemyState;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EdwardState.Idle);
    }

    private void Update()
    {
        // 🔥 Auto-reconnect do player (necessário por DontDestroyOnLoad)
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

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

        bool currentIsAttacking = anim.GetBool("isAttacking");
        if (currentIsAttacking != lastIsAttackingState)
        {
            Debug.Log("isAttacking mudou para: " + currentIsAttacking);
            lastIsAttackingState = currentIsAttacking;
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

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    // ============================================================================================
    // 🔥 ALTERAÇÃO AQUI — BossAttack agora tem carregamento antes de causar dano
    // ============================================================================================

    public void BossAttack()
    {
        Debug.Log("EdwardMovement: BossAttack() via animação — iniciando carregamento...");
        ChangeState(EdwardState.Attacking);
        StartCoroutine(DelayedAttack());
    }

    private System.Collections.IEnumerator DelayedAttack()
    {
        yield return new WaitForSeconds(0.25f); // 👈 TEMPO DE CARREGAMENTO DO ATAQUE
        GetComponent<EnemyCombat>()?.Attack();
    }

    // ============================================================================================

    public void EndBossAttack()
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

    public void ChangeState(EdwardState newState)
    {
        enemyState = newState;

        if (anim != null)
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
}

public enum EdwardState
{
    Idle,
    Chasing,
    Attacking
}
