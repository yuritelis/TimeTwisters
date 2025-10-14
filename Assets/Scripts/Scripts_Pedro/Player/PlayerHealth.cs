using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int currentHealth;
    public int maxHealth;

    [Header("Invencibilidade")]
    public bool isInvincible = false;
    public float invincibilityDuration = 1f;
    public float flashInterval = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 && isInvincible) return;

        currentHealth += amount;

        if (amount < 0)
        {
            StartCoroutine(InvincibilityFrames());
        }

        if (currentHealth <= 0)
        {
            if (gameObject.CompareTag("Player"))
                SceneManager.LoadScene("DeathScreen");
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                // Pisca usando alpha
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
                yield return new WaitForSeconds(flashInterval);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashInterval);
            }

            elapsed += flashInterval * 2;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isInvincible = false;
    }

}
