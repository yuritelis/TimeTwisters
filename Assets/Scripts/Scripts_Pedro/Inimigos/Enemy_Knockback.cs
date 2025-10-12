using UnityEngine;
using System.Collections;

public class Enemy_Knockback : MonoBehaviour
{
    private bool isKnocked = false;

    public void Knockback(Transform playerTransform, float knockbackDistance, float knockbackDuration = 0.1f)
    {
        if (!isKnocked)
        {
            StartCoroutine(ApplyKnockback(playerTransform, knockbackDistance, knockbackDuration));
        }
    }

    private IEnumerator ApplyKnockback(Transform playerTransform, float distance, float duration)
    {
        isKnocked = true;

        Vector2 start = transform.position;
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        Vector2 target = start + direction * distance;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isKnocked = false;
    }

    public bool IsKnocked()
    {
        return isKnocked;
    }
}
