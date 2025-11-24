using UnityEngine;

public class EdwardMovement : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float playerDetectRange = 5f;

    public Transform attackPoint;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Animator anim;
    public Transform player;

    private EdwardState enemyState;
    public bool canMove = true;

    public EdwardState CurrentState => enemyState;
    public bool allowAutoAttack = false;

    private bool lastIsAttackingState = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EdwardState.Idle);
    }

    private void Update()
    {
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

        CheckForPlayer();

        switch (enemyState)
        {
            case EdwardState.Chasing:
                if (player != null)
                {
                    float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);
                    if (distanceToPlayer > attackRange)
                        Chase();
                    else
                        rb.linearVelocity = Vector2.zero;
                }
                break;

            case EdwardState.Attacking:
                rb.linearVelocity = Vector2.zero;
                break;

            case EdwardState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
        }

        bool currentIsAttacking = anim.GetBool("isAttacking");
        if (currentIsAttacking != lastIsAttackingState)
            lastIsAttackingState = currentIsAttacking;
    }

    private void CheckForPlayer()
    {
        if (allowAutoAttack) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            if (enemyState != EdwardState.Attacking)
                ChangeState(EdwardState.Chasing);
        }
        else
        {
            if (enemyState != EdwardState.Attacking)
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

    public void TriggerAttackAnimation()
    {
        ChangeState(EdwardState.Attacking);
        anim.SetBool("isAttacking", true);
        canMove = false;
    }

    public void EndNormalAttackAnimation()
    {
        anim.SetBool("isAttacking", false);
        ChangeState(EdwardState.Chasing);
        canMove = true;
    }

    public void ChangeState(EdwardState newState)
    {
        enemyState = newState;

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

public enum EdwardState
{
    Idle,
    Chasing,
    Attacking
}
