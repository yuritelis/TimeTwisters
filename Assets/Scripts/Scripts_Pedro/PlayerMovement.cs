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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Captura do input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized;

        // Se não estiver se movendo
        if (input == Vector2.zero)
        {
            isWalking = false;
            animator.SetBool("isWalking", false);

            // Salva a última direção no Animator
            animator.SetFloat("LastInputX", lastInput.x);
            animator.SetFloat("LastInputY", lastInput.y);
        }
        else
        {
            // Se estiver se movendo
            isWalking = true;
            animator.SetBool("isWalking", true);

            animator.SetFloat("InputX", input.x);
            animator.SetFloat("InputY", input.y);

            // Atualiza última direção
            lastInput = input;
        }

        // Flip no sprite
        if (input.x < 0) GetComponent<SpriteRenderer>().flipX = true;
        else if (input.x > 0) GetComponent<SpriteRenderer>().flipX = false;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;

        Vector3 lookDir;

        if (isWalking)
        {
            // Olha para onde está se movendo
            lookDir = new Vector3(input.x, input.y, 0);
        }
        else
        {
            // Olha para a última direção de movimento
            lookDir = new Vector3(lastInput.x, lastInput.y, 0);
        }
    }
}
