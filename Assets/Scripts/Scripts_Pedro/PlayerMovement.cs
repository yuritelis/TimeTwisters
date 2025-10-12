using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float speed = 0.5f;
    private Rigidbody2D rb;
    private Vector2 input;
    private Animator animator;
    private bool isWalking = false;

    private Vector2 lastInput = Vector2.down;
    public Vector2 LastInput => lastInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized;

        bool isAttacking = animator != null && animator.GetBool("isAttacking");

        if (input != Vector2.zero)
        {
            lastInput = input;
        }

        isWalking = input != Vector2.zero;
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);

            if (!isAttacking)
            {
                animator.SetFloat("InputX", input.x);
                animator.SetFloat("InputY", input.y);
                animator.SetFloat("LastInputX", lastInput.x);
                animator.SetFloat("LastInputY", lastInput.y);
            }
        }

        // Flip no sprite
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (lastInput.x < 0) sr.flipX = true;
            else if (lastInput.x > 0) sr.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.linearVelocity = input * speed;
    }
}
