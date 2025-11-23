using UnityEngine;

public class ElLoboAnimationTest : MonoBehaviour
{
    private Animator anim;
    private EdwardMovement eMove;

    private float moveX = 0;
    private float moveY = -1;

    void Start()
    {
        anim = GetComponent<Animator>();
        eMove = GetComponent<EdwardMovement>();

        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", true);
        anim.SetBool("isAttacking", false);
    }

    private void Update()
    {
        if (eMove == null)
            return;

        EdwardState eState = eMove.CurrentState;
        bool isWalking = eMove.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 0.1f;

        if (eState == EdwardState.Chasing)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isIdle", false);
            anim.SetBool("isAttacking", false);
        }
        else if (eState == EdwardState.Attacking)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isIdle", false);
            anim.SetBool("isAttacking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isIdle", true);
            anim.SetBool("isAttacking", false);
        }

        if ((eState == EdwardState.Attacking || isWalking) && eMove.player != null)
        {
            Vector3 direction = (eMove.player.position - transform.position).normalized;

            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);

            moveX = direction.x;
            moveY = direction.y;
        }

        anim.SetFloat("LastMoveX", moveX);
        anim.SetFloat("LastMoveY", moveY);
    }
}
