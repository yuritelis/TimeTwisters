using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossEdwardController : MonoBehaviour
{
    public Transform player;

    public float attackInterval = 5f;
    public float attackRange = 6f;

    public BossEdward_Leap_Attack leapAttack;
    public BossEdward_Claw_Attack clawAttack;

    public EnemyCombat normalAttack;
    public float normalAttackRange = 2f;
    public float normalAttackCooldown = 1.2f;

    private bool isAttackingSpecial = false;
    private EdwardMovement movement;
    private bool canDoNormalAttack = true;

    public bool isDead = false;

    public Enemy_Health bossHp;
    private int lastHp;

    void Awake()
    {
        movement = GetComponent<EdwardMovement>();
        bossHp = GetComponent<Enemy_Health>();

        if (bossHp != null)
            lastHp = bossHp.currentHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (movement != null)
            movement.allowAutoAttack = false;
    }

    void Start()
    {
        StartCoroutine(AttackCycle());
        StartCoroutine(NormalAttackLoop());
    }

    void Update()
    {
        if (isDead) return;
        if (bossHp == null) return;

        lastHp = bossHp.currentHealth;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        leapAttack?.CleanupAfterDeath();
        clawAttack?.CleanupAfterDeath();
    }

    IEnumerator NormalAttackLoop()
    {
        while (!isDead)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
                movement.player = player;
                yield return null;
                continue;
            }

            if (!isAttackingSpecial && canDoNormalAttack)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= normalAttackRange)
                {
                    movement.canMove = false;
                    movement.TriggerAttackAnimation();

                    canDoNormalAttack = false;
                    yield return new WaitForSeconds(normalAttackCooldown);
                    canDoNormalAttack = true;
                }
            }

            yield return null;
        }
    }

    IEnumerator AttackCycle()
    {
        yield return new WaitForSeconds(2f);

        while (!isDead)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
                movement.player = player;
                yield return null;
                continue;
            }

            float distance = Vector2.Distance(transform.position, player.position);

            if (!isAttackingSpecial && distance <= attackRange)
            {
                yield return StartCoroutine(ChooseRandomAttack());
                yield return new WaitForSeconds(attackInterval);
            }

            yield return null;
        }
    }

    IEnumerator ChooseRandomAttack()
    {
        isAttackingSpecial = true;
        movement.canMove = false;

        int attackIndex = Random.Range(0, 2);

        switch (attackIndex)
        {
            case 0:
                if (leapAttack != null)
                    yield return StartCoroutine(leapAttack.DoLeap(player, this));
                break;

            case 1:
                if (clawAttack != null)
                    yield return StartCoroutine(clawAttack.SpawnClawsCoroutine(this));
                break;
        }

        movement.canMove = true;
        isAttackingSpecial = false;
    }
}
