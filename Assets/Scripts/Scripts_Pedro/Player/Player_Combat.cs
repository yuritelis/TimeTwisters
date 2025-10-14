using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    private PlayerController playerMovement;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public Animator anim;
    private SpriteRenderer sr;
    private Vector3 originalAttackPointLocalPos;

    void Start()
    {
        playerMovement = GetComponent<PlayerController>();
        anim = anim != null ? anim : GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (attackPoint != null)
            originalAttackPointLocalPos = attackPoint.localPosition;
    }

    public void Attack()
    {
        if (anim.GetBool("isAttacking"))
            return;

        Vector2 dir = playerMovement != null ? playerMovement.LastInput : Vector2.right;

        if (attackPoint != null)
            attackPoint.localPosition = dir.normalized * weaponRange;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            sr.flipX = dir.x < 0;

        anim.SetFloat("InputX", dir.x);
        anim.SetFloat("InputY", dir.y);
        anim.SetFloat("LastInputX", dir.x);
        anim.SetFloat("LastInputY", dir.y);
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (attackPoint == null)
            return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            var h = enemy.GetComponent<Enemy_Health>();
            var k = enemy.GetComponent<Enemy_Knockback>();

            if (h != null)
                h.ChangeHealth(-damage);

            if (k != null)
                k.Knockback(transform, 0.5f, 0.1f);
        }
    }

    public void FinishAttacking()
    {
        if (anim != null)
            anim.SetBool("isAttacking", false);

        if (attackPoint != null)
            attackPoint.localPosition = originalAttackPointLocalPos;

        sr.flipX = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }

    private void UpdateSpriteFlip(Vector2 direction)
    {
        if (sr == null)
            return;

        if (direction.x < 0)
            sr.flipX = true;
        else if (direction.x > 0)
            sr.flipX = false;
    }
}
