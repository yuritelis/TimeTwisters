using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public Transform attackPoint;
    public float weaponRange = 1;
    public float knockbackForce = 0.5f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public Animator anim;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Attack()
    {
        if (anim == null)
        {
            Debug.LogWarning("Player_Combat: Animator não atribuído.");
            return;
        }

        if (anim.GetBool("isAttacking")) return;

        Vector2 currentInput;
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");
        currentInput = currentInput.normalized;

        Vector2 dir = playerMovement != null ? playerMovement.LastInput : Vector2.right;

        if (currentInput != Vector2.zero)
        {
            dir = currentInput;
        }

        if (attackPoint != null)
        {
            attackPoint.localPosition = dir.normalized * weaponRange;
        }

        anim.SetFloat("LastInputX", dir.x);
        anim.SetFloat("LastInputY", dir.y);
        anim.SetFloat("InputX", dir.x);
        anim.SetFloat("InputY", dir.y);

        Debug.Log($"Player_Combat.Attack() -> dir={dir} | Animator LastInputX={anim.GetFloat("LastInputX")} LastInputY={anim.GetFloat("LastInputY")}");

        anim.SetBool("isAttacking", true);
        anim.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            var h = enemies[0].GetComponent<Enemy_Health>();
            var k = enemies[0].GetComponent<Enemy_Knockback>();

            if (h != null) h.ChangeHealth(-damage);
            if (k != null) k.Knockback(transform, knockbackForce, 0.1f);
        }
    }

    public void FinishAttacking()
    {
        if (anim != null) anim.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
