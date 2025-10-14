using UnityEngine;
using System.Collections;

public class Enemy_Knockback : MonoBehaviour
{
    [Header("Resist�ncia ao knockback")]
    [Range(0f, 1f)]
    public float knockbackResistance = 0f; // 0 = sem resist�ncia, 1 = totalmente imune

    private bool isKnocked = false;

    public void Knockback(Transform playerTransform, float knockbackDistance, float knockbackDuration = 0.1f)
    {
        if (!isKnocked)
        {
            // Reduz a for�a de acordo com a resist�ncia
            float adjustedDistance = knockbackDistance * (1f - knockbackResistance);
            StartCoroutine(KnockbackCoroutine(playerTransform, adjustedDistance, knockbackDuration));
        }
    }

    private IEnumerator KnockbackCoroutine(Transform playerTransform, float distance, float duration)
    {
        isKnocked = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (transform.position - playerTransform.position).normalized;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                rb.MovePosition(rb.position + dir * (distance / duration) * Time.fixedDeltaTime);
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        isKnocked = false;
    }
}
