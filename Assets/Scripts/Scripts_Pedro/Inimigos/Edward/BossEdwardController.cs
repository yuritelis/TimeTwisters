using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossEdwardController : MonoBehaviour
{
    [Header("Refer�ncias")]
    public Transform player;

    [Header("Configura��es Gerais")]
    public float attackInterval = 5f;
    public float attackRange = 6f;

    [Header("Ataques")]
    public BossEdward_Leap_Attack leapAttack;
    public BossEdward_Wave_Attack waveAttack;
    public BossEdward_Claw_Attack clawAttack;

    private bool isAttacking = false;
    private SpriteRenderer sr;
    private EdwardMovement movement;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<EdwardMovement>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Start()
    {
        StartCoroutine(AttackCycle());
    }

    IEnumerator AttackCycle()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (!isAttacking && player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= attackRange)
                {
                    yield return StartCoroutine(ChooseRandomAttack());
                    yield return new WaitForSeconds(attackInterval);
                }
                else yield return null;
            }
            else yield return null;
        }
    }

    IEnumerator ChooseRandomAttack()
    {
        isAttacking = true;
        if (movement != null) movement.canMove = false;

        int attackIndex = Random.Range(0, 3);

        switch (attackIndex)
        {
            case 0: // Leap
                if (leapAttack != null)
                    yield return StartCoroutine(leapAttack.DoLeap(player));
                break;

            case 1: // Wave
                if (waveAttack != null)
                    yield return StartCoroutine(waveAttack.DoWave());
                break;

            case 2: // Claw
                if (clawAttack != null)
                    clawAttack.SpawnClaws(); // n�o bloqueia boss, paralelo
                break;
        }

        if (movement != null) movement.canMove = true;
        isAttacking = false;
    }
}
