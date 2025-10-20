using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float knockbackDecay = 12f;

    public bool canMove = true; // ✅ controle de movimentação (usado pelo Stealth)

    private Rigidbody2D rb;
    private Vector2 input;
    private Animator anim;
    public Player_Combat player_Combat;
    private bool isKnockedBack;
    private Vector2 lastInput = Vector2.right;

    public Vector2 LastInput => lastInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // pausa da timeline
        if (TimelineUI.isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ❌ se o player estiver travado (por stealth, cutscene etc.)
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            return;
        }

        // knockback ativo
        if (!isKnockedBack)
        {
            rb.linearVelocity = input * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, knockbackDecay * Time.deltaTime);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        // evita movimentar quando travado, pausado ou em knockback
        if (!canMove || TimelineUI.isPaused || isKnockedBack)
        {
            anim.SetBool("isWalking", false);
            input = Vector2.zero;
            return;
        }

        anim.SetBool("isWalking", !context.canceled);

        if (context.canceled)
        {
            anim.SetFloat("LastInputX", lastInput.x);
            anim.SetFloat("LastInputY", lastInput.y);
            input = Vector2.zero;
        }
        else
        {
            input = context.ReadValue<Vector2>();

            if (input != Vector2.zero)
                lastInput = input.normalized;
        }

        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!canMove || TimelineUI.isPaused || isKnockedBack)
            return;

        if (context.performed)
        {
            player_Combat.Attack();
        }
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;

        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;

        StartCoroutine(KnockbackCounter(stunTime));
    }

    private IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
}
