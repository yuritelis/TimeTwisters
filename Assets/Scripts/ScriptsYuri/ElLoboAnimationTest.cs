using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class FollowPlayer : MonoBehaviour
{
    private Animator anim;

    private EnemyState eState;
    
    private Enemy_Movement eMove;

    private bool isWalking = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        anim.SetBool("isWalking", false);
        anim.SetBool("isChasing", false);
        anim.SetBool("isIdle", true);
        anim.SetBool("isAttacking", false);
    }

    private void Update()
    {
        if (eState != EnemyState.Idle && eState != EnemyState.Attacking)
        {
            isWalking = true;
            anim.SetBool("isWalking", true);
            anim.SetBool("isChasing", true);
            anim.SetBool("isIdle", false);
            anim.SetBool("isAttacking", false);
        }
        else if (eState != EnemyState.Chasing && eState != EnemyState.Idle)
        {
            isWalking = false;
            anim.SetBool("isWalking", false);
            anim.SetBool("isChasing", false);
            anim.SetBool("isIdle", false);
            anim.SetBool("isAttacking", true);
        }
        else
        {
            isWalking = false;
            anim.SetBool("isWalking", false);
            anim.SetBool("isChasing", false);
            anim.SetBool("isIdle", true);
            anim.SetBool("isAttacking", false);
        }

        if(eMove.transform.position.x > eMove.player.position.x && isWalking)
        {
            anim.SetFloat("InputX", pos.x);
            anim.SetFloat("InputY", 0);
        }
        else if (transform.position.x < -0.0001f && isWalking)
        {
            anim.SetFloat("InputX", pos.x);
            anim.SetFloat("InputY", 0);
        }

        if (transform.position.y > 0.001f && isWalking)
        {
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", pos.y);
        }
        else if (transform.position.y < -0.0001f && isWalking)
        {
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", pos.y);
        }

        anim.SetFloat("LastInputX", pos.x);
        anim.SetFloat("LastInputY", pos.y);
    }
}
