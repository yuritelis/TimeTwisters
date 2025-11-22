using UnityEngine;
using System.Collections;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public float hitFlashDuration = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (amount < 0)
            StartHitFlash();

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            GetComponent<EnemyPersistence>()?.MarkAsDead();
            GetComponent<EnemyDeathDialogTrigger>()?.OnEnemyDefeated();
            Destroy(gameObject);
        }
    }

    void StartHitFlash()
    {
        if (spriteRenderer == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(HitFlash());
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }
}
