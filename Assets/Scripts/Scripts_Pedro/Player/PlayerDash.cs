using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (inputDir != Vector2.zero)
        {
            if (Mathf.Abs(inputDir.x) == 0 || Mathf.Abs(inputDir.y) == 0)
                lastCardinalDirection = inputDir.normalized;
        }

        // Dash
        if (canDash && !isDashing && Input.GetKeyDown(dashKey))
        {
            Vector2 dashDir;

            if (inputDir != Vector2.zero)
                dashDir = inputDir.normalized;
            else
                dashDir = lastCardinalDirection;

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
