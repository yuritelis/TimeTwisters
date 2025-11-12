using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimatorController : MonoBehaviour
{
    private Animator anim;

    // Flags públicas que outros scripts (como o Enemy_Movement) podem alterar
    public bool isIdle;
    public bool isWalking;
    public bool isAttacking;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Atualiza o Animator a cada frame conforme os flags
        anim.SetBool("isIdle", isIdle);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isAttacking", isAttacking);
    }

    // Métodos auxiliares para controle direto de estados
    public void SetIdle()
    {
        isIdle = true;
        isWalking = false;
        isAttacking = false;
        UpdateAnimator();
    }

    public void SetWalk()
    {
        isIdle = false;
        isWalking = true;
        isAttacking = false;
        UpdateAnimator();
    }

    public void SetAttack()
    {
        isIdle = false;
        isWalking = false;
        isAttacking = true;
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        anim.SetBool("isIdle", isIdle);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isAttacking", isAttacking);
    }
}
