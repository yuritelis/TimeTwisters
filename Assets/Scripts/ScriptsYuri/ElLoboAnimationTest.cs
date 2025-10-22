using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class ElLoboAnimationTest : MonoBehaviour
{
    private Animator anim;

    private EnemyState eState;

    private EdwardMovement eMove;

    private bool isWalking;
    private float moveX = 0;
    private float moveY = -1;

    void Start()
    {
        anim = GetComponent<Animator>();
        eMove = GetComponent<EdwardMovement>();

        isWalking = false;

        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", true);
        anim.SetBool("isAttacking", false);
    }

    private void Update()
    {
        isWalking = eMove.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 0.1f;

        if (eState == EnemyState.Chasing)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isIdle", false);
            anim.SetBool("isAttacking", false);
        }
        else if (eState == EnemyState.Attacking)
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

        if (isWalking)
        {
            Vector3 direction = (eMove.player.position - transform.position).normalized;

            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", true);
            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);

            moveX = direction.x;
            moveY = direction.y;
        }

        anim.SetFloat("LastMoveX", moveX);
        anim.SetFloat("LastMoveY", moveY);
    }
}