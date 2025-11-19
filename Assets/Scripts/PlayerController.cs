using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float knockbackDecay = 12f;

    [Header("Controle Geral")]
    public bool canMove = true;

    [Header("Referências")]
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInput playerInput;
    private Vector2 input;
    public Player_Combat player_Combat;

    private bool isKnockedBack;
    private Vector2 lastInput = Vector2.right;
    public Vector2 LastInput => lastInput;

    public bool IsAttacking => anim.GetBool("isAttacking");
    public bool IsHit
    {
        get
        {
            var state = anim.GetCurrentAnimatorStateInfo(0);
            return state.IsName("Hit") || state.IsName("Player_Hit");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        PauseController.SetPause(false);
    }

    void Update()
    {
        if (TimelineUI.isPaused || PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        bool isAttacking = anim.GetBool("isAttacking");
        bool isHit = IsHit;

        if (!canMove || isAttacking || isHit)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            return;
        }

        input = playerInput.actions["Move"].ReadValue<Vector2>();

        if (input != Vector2.zero)
        {
            anim.SetBool("isWalking", true);
            lastInput = input.normalized;

            if (!isAttacking)
            {
                anim.SetFloat("InputX", input.x);
                anim.SetFloat("InputY", input.y);
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (!isKnockedBack)
            rb.linearVelocity = input * moveSpeed;
        else
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, knockbackDecay * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        bool isAttacking = anim.GetBool("isAttacking");

        if (context.canceled && !isAttacking)
        {
            anim.SetFloat("LastInputX", lastInput.x);
            anim.SetFloat("LastInputY", lastInput.y);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        bool isAttacking = anim.GetBool("isAttacking");
        bool isHit = IsHit;

        if (!canMove || TimelineUI.isPaused || PauseController.IsGamePaused || isKnockedBack || isAttacking || isHit)
            return;

        if (context.performed)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            input = Vector2.zero;

            player_Combat.Attack();

            StartCoroutine(LockMovementUntilAttackStarts());
        }
    }

    private IEnumerator LockMovementUntilAttackStarts()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            rb.linearVelocity = Vector2.zero;
            yield return null;
        }

        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            yield return null;
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

    // -------------------------------------------------------------------------
    //  NOVA FUNCIONALIDADE — F10 multiplica moveSpeed por 3 (SÓ NA UNITY)
    // -------------------------------------------------------------------------
    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.f10Key.wasPressedThisFrame)
        {
            moveSpeed *= 3f;
            Debug.Log($"[DEBUG SPEED] F10 — moveSpeed agora = {moveSpeed}");
        }
#endif
    }
}
