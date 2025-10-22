using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    private PlayerController playerMovement;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public Animator anim;
    private Vector3 originalAttackPointLocalPos;

    private Vector2 attackDirection;

    void Start()
    {
        playerMovement = GetComponent<PlayerController>();
        anim = anim != null ? anim : GetComponent<Animator>();

        if (attackPoint != null)
            originalAttackPointLocalPos = attackPoint.localPosition;
    }

    public void Attack()
    {
        if (anim.GetBool("isAttacking"))
            return;

        // 🧭 pega a direção atual e "discretiza" pra uma das 4 direções fixas
        Vector2 dir = playerMovement != null ? playerMovement.LastInput.normalized : Vector2.right;
        attackDirection = GetCardinalDirection(dir);

        if (attackPoint != null)
            attackPoint.localPosition = attackDirection * weaponRange;

        // 🔒 usa apenas a direção fixa, sem variação
        anim.SetFloat("InputX", attackDirection.x);
        anim.SetFloat("InputY", attackDirection.y);
        anim.SetFloat("LastInputX", attackDirection.x);
        anim.SetFloat("LastInputY", attackDirection.y);

        anim.SetBool("isAttacking", true);
        anim.SetTrigger("Attack");
    }

    // ✅ força o ataque pra uma direção exata (sem diagonais)
    private Vector2 GetCardinalDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0); // direita/esquerda
        else
            return new Vector2(0, Mathf.Sign(dir.y)); // cima/baixo
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
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
