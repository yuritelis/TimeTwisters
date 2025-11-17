using UnityEngine;
using System.Collections;

public class Enemy_Knockback : MonoBehaviour
{
    [Header("Resistência ao knockback")]
    [Range(0f, 1f)] public float knockbackResistance = 0f;

    private bool isKnocked = false;

    public void Knockback(Transform playerTransform, float knockbackDistance, float knockbackDuration = 0.1f)
    {
        if (!isActiveAndEnabled) return;

        if (!isKnocked)
        {
            float adjustedDistance = knockbackDistance * (1f - knockbackResistance);
            StartCoroutine(KnockbackCoroutine(playerTransform, adjustedDistance, knockbackDuration));
        }
    }

    private IEnumerator KnockbackCoroutine(Transform playerTransform, float distance, float duration)
    {
        isKnocked = true;

        var movementScript = GetComponent<Enemy_Movement>();
        if (movementScript != null)
            movementScript.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            var movement = GetComponent<Enemy_Movement>();
            Transform source = movement != null && movement.attackPoint != null
                ? movement.attackPoint
                : transform;

            Vector2 dir = (source.position - playerTransform.position).normalized;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                rb.MovePosition(rb.position + dir * (distance / duration) * Time.fixedDeltaTime);
                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        if (movementScript != null)
            movementScript.enabled = true;

        isKnocked = false;
    }
}
