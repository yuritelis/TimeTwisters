using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public KeyCode dashKey = KeyCode.LeftShift;

    private bool isDashing = false;
    private bool canDash = true;

    private Rigidbody2D rb;
    private Vector2 lastCardinalDirection = Vector2.right;

    private PlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private PlayerInput playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name != "TitleScreen")
        {
            isDashing = false;
            canDash = true;
            if (playerHealth != null)
                playerHealth.isInvincible = false;
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;
        }
    }

    private void Update()
    {
        if (playerController != null && !playerController.canMove)
            return;

        Vector2 inputDir = playerInput.actions["Move"].ReadValue<Vector2>();

        if (inputDir.sqrMagnitude > 0.1f)
            lastCardinalDirection = inputDir.normalized;

        if (playerController != null)
        {
            if (playerController.IsAttacking || playerController.IsHit)
                return;
        }

        if (canDash && !isDashing && Input.GetKeyDown(dashKey))
        {
            Vector2 dashDir = (inputDir.sqrMagnitude > 0.1f) ? inputDir.normalized : lastCardinalDirection;
            StartCoroutine(PerformDash(dashDir));
        }
    }

    private IEnumerator PerformDash(Vector2 dashDirection)
    {
        isDashing = true;
        canDash = false;

        if (playerHealth != null)
            playerHealth.isInvincible = true;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.6f, 0.8f, 1f, 1f);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        if (playerHealth != null)
            playerHealth.isInvincible = false;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
