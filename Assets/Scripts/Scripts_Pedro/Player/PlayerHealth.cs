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
    private FogController fog; // 🌫️

    private float lastHealth = -1f;

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

        // 🧠 Atualiza névoa inicial (vida cheia ou valor já definido)
        if (fog != null)
        {
            float sanityPercent = (float)currentHealth / maxHealth;
            fog.UpdateFog(sanityPercent);
        }

        lastHealth = currentHealth;
    }

    private void Update()
    {
        // 🔹 Atualiza fog automaticamente se a vida mudar (debug, inspector, cheat etc.)
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

            // 🔻 Checa morte imediata se foi zerada manualmente
            if (currentHealth <= 0 && gameObject.CompareTag("Player"))
                SceneManager.LoadScene("DeathScreen");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 && isInvincible) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"[PlayerHealth] ChangeHealth chamado. Amount: {amount}, CurrentHealth: {currentHealth}");

        if (ui != null)
            ui.UpdateVidas(currentHealth);

        // 🌫️ Atualiza fog conforme a nova sanidade
        if (fog != null)
        {
            float sanityPercent = (float)currentHealth / maxHealth;
            fog.UpdateFog(sanityPercent);
        }

        if (amount < 0)
            StartCoroutine(InvincibilityFrames());

        if (currentHealth <= 0 && gameObject.CompareTag("Player"))
            SceneManager.LoadScene("DeathScreen");
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
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

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            false
        );

        isInvincible = false;
    }
}
