using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    private bool isInvincible = false;
    public float invincibilityDuration = 1f;

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
            Destroy(gameObject);
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
