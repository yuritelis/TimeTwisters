using UnityEngine;

public class ClawDamage : MonoBehaviour
{
    [Header("Dano")]
    public int damage = 1;
    public float hitboxDuration = 0.1f; // opcional: tempo até destruir a garra depois de acertar

    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;

            // Aqui usamos ChangeHealth passando valor negativo
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage);
            }

            // destruir a garra depois de um pequeno delay
            Destroy(gameObject, hitboxDuration);
        }
    }
}
