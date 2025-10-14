using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossEdwardController : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações Gerais")]
    public float attackInterval = 5f;
    public float attackRange = 6f;

    [Header("Ataques")]
    public BossEdward_Leap_Attack leapAttack;
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
        yield return new WaitForSeconds(2f); // atraso inicial

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

        // Sorteia apenas entre Leap (0) e Claw (1)
        int attackIndex = Random.Range(0, 2);
        Debug.Log($"[Boss] Ataque sorteado: {attackIndex} ({(attackIndex == 0 ? "Leap" : "Claw")})");

        switch (attackIndex)
        {
            case 0: // Leap
                if (leapAttack != null)
                {
                    Debug.Log("[Boss] Executando Leap");
                    yield return StartCoroutine(leapAttack.DoLeap(player));
                }
                break;
            case 1: // Claw
                if (clawAttack != null)
                {
                    Debug.Log("[Boss] Executando Claw");
                    yield return StartCoroutine(clawAttack.SpawnClawsCoroutine());
                }
                break;
        }

        if (movement != null) movement.canMove = true;
        isAttacking = false;
    }
}
