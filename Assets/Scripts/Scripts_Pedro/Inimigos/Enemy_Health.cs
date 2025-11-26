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

    private bool levouDanoReal = false;
    public bool LevouDanoReal => levouDanoReal;

    private AvancaEtapaAoMorrer avancaEtapa;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        avancaEtapa = GetComponent<AvancaEtapaAoMorrer>();
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
            levouDanoReal = true;

        currentHealth += amount;

        if (amount < 0)
            StartHitFlash();

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            if (levouDanoReal)
            {
                GetComponent<EnemyPersistence>()?.MarkAsDead();
                GetComponent<EnemyDeathDialogTrigger>()?.OnEnemyDefeated();
                if (avancaEtapa != null)
                    avancaEtapa.ForcarAvanco();
            }

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
