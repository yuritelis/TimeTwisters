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

    [Header("Ataque Normal")]
    public EnemyCombat normalAttack;
    public float normalAttackRange = 2f;
    public float normalAttackCooldown = 1f;

    private bool isAttackingSpecial = false;
    private SpriteRenderer sr;
    private EdwardMovement movement;
    private bool canDoNormalAttack = true; // Novo: controle específico para ataques normais

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<EdwardMovement>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Start()
    {
        StartCoroutine(AttackCycle());       // ataques especiais
        StartCoroutine(NormalAttackLoop());  // ataques normais contínuos
    }

    IEnumerator NormalAttackLoop()
    {
        while (true)
        {
            if (!isAttackingSpecial && player != null && canDoNormalAttack)
            {
                float distance = Vector2.Distance(transform.position, player.position);
                if (distance <= normalAttackRange)
                {
                    // Executa ataque normal através do EdwardMovement para ter a animação
                    if (movement != null)
                    {
                        canDoNormalAttack = false;

                        // Aguarda cooldown específico para ataques normais
                        yield return new WaitForSeconds(normalAttackCooldown);
                        canDoNormalAttack = true;
                    }
                }
            }

            yield return null; // espera o próximo frame
        }
    }

    IEnumerator AttackCycle()
    {
        yield return new WaitForSeconds(2f); // atraso inicial

        while (true)
        {
            if (!isAttackingSpecial && player != null)
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
        isAttackingSpecial = true;
        if (movement != null) movement.canMove = false;

        int attackIndex = Random.Range(0, 2);
        Debug.Log($"[Boss] Ataque especial sorteado: {attackIndex}");

        switch (attackIndex)
        {
            case 0:
                if (leapAttack != null)
                    yield return StartCoroutine(leapAttack.DoLeap(player));
                break;
            case 1:
                if (clawAttack != null)
                    yield return StartCoroutine(clawAttack.SpawnClawsCoroutine());
                break;
        }

        if (movement != null) movement.canMove = true;
        isAttackingSpecial = false;
    }
}