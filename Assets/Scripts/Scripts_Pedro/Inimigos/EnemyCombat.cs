using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float attackRange;
    public LayerMask playerLayer;

    private bool isAttacking = false;

    public void Attack()
    {
        isAttacking = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        if (hits.Length > 0)
        {
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
            HitStopManager.Instance.DoGlobalHitStop(0.08f);
            Debug.Log("Player levou dano");
        }
    }


    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
