using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float attackRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    private bool isAttacking = false;

    public void Attack()
    {
        isAttacking = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        // Se tiver mais de 1 elemento no array, significa que o jogador está dentro do attackPoint (ou seja, no alcance do ataque).
        if (hits.Length > 0)
        {
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
            hits[0].GetComponent<PlayerController>().Knockback(transform, knockbackForce, stunTime);
            Debug.Log("Player levou dano");
        }
    }

    private void StopAttacking()
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
