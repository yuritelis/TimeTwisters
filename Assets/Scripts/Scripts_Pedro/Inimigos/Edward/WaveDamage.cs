using UnityEngine;

public class WaveDamage : MonoBehaviour
{
    public int damage = 2;
    public LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            other.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
        }
    }
}
