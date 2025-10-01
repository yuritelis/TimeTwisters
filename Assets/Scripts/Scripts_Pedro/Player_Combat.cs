using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }
}
