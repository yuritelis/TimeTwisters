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

    public VidaUI ui;
    private FogController fog;

    private float lastHealth = -1f;

    public float healFlashDuration = 0.3f;
    public Color healColor = new Color(0.6f, 1f, 0.6f, 1f);

    private bool isHealingFlash = false;
    private bool isDamageFlashing = false;

    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        spriteRenderer = GetComponent<SpriteRenderer>();
        ui = FindFirstObjectByType<VidaUI>();
        fog = FindFirstObjectByType<FogController>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (ui != null)
        {
            ui.SetVidaMax(maxHealth);
            ui.UpdateVidas(currentHealth);
        }

        if (fog != null)
        {
            float sanityPercent = (float)currentHealth / maxHealth;
            fog.UpdateFog(sanityPercent);
        }

        lastHealth = currentHealth;
    }

    private void Update()
    {
        if (currentHealth != lastHealth)
        {
            lastHealth = currentHealth;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (ui != null)
                ui.UpdateVidas(currentHealth);

            if (fog != null)
            {
                float sanityPercent = (float)currentHealth / maxHealth;
                fog.UpdateFog(sanityPercent);
            }

            if (currentHealth <= 0 && gameObject.CompareTag("Player"))
                SceneManager.LoadScene("DeathScreen");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 && isInvincible) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (ui != null)
            ui.UpdateVidas(currentHealth);

        if (fog != null)
        {
            float sanityPercent = (float)currentHealth / maxHealth;
            fog.UpdateFog(sanityPercent);
        }

        if (amount > 0)
            StartCoroutine(HealFlash());

        if (amount < 0)
            StartCoroutine(InvincibilityFrames());

        if (currentHealth <= 0 && gameObject.CompareTag("Player"))
            SceneManager.LoadScene("DeathScreen");
    }

    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        isInvincible = false;

        if (ui != null)
            ui.UpdateVidas(currentHealth);

        if (fog != null)
            fog.UpdateFog(1f);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private IEnumerator HealFlash()
    {
        if (spriteRenderer == null) yield break;
        if (isDamageFlashing || isInvincible) yield break;

        isHealingFlash = true;

        float t = 0f;
        while (t < healFlashDuration)
        {
            float lerp = t / healFlashDuration;
            spriteRenderer.color = Color.Lerp(originalColor, healColor, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = healColor;

        t = 0f;
        while (t < healFlashDuration)
        {
            float lerp = t / healFlashDuration;
            spriteRenderer.color = Color.Lerp(healColor, originalColor, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        if (!isDamageFlashing && !isInvincible)
            spriteRenderer.color = originalColor;

        isHealingFlash = false;
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        isDamageFlashing = true;

        float elapsed = 0f;

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            true
        );

        while (elapsed < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
                yield return new WaitForSeconds(flashInterval);

                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashInterval);
            }
            elapsed += flashInterval * 2;
        }

        if (spriteRenderer != null && !isHealingFlash)
            spriteRenderer.color = originalColor;

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            false
        );

        isDamageFlashing = false;
        isInvincible = false;
    }
}
