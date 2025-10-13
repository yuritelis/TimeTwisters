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
    public float attackRange = 5f; // alcance máximo do ataque

    [Header("Ataque Leap")]
    public BossEdward_Leap_Attack leapAttack;

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
            if (!isAttacking)
            {
                // Só ataca se o player estiver dentro do alcance definido
                if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
                {
                    yield return StartCoroutine(DoLeapAttack());
                    yield return new WaitForSeconds(attackInterval);
                }
                else
                {
                    yield return null;
                }
            }
            else
                yield return null;
        }
    }

    IEnumerator DoLeapAttack()
    {
        if (player == null || leapAttack == null) yield break;

        // Checa novamente antes de iniciar
        if (Vector2.Distance(transform.position, player.position) > attackRange)
            yield break;

        isAttacking = true;

        if (movement != null) movement.canMove = false;

        // chama o ataque completo (telegraph + dash)
        yield return StartCoroutine(leapAttack.DoLeap(player));

        if (movement != null) movement.canMove = true;

        isAttacking = false;
    }
}
