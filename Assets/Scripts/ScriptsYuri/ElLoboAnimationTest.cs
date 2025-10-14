using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] GameObject target;

    Rigidbody2D rb;
    Animator anim;
    private Vector2 input;

    private bool isWalking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        anim.SetBool("isWalking", false);
        isWalking = false;
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 dif = target.transform.position - transform.position;

            rb.AddForce(dif);
            anim.SetBool("isWalking", true);
            isWalking = true;
        }
        else
        {
            anim.SetBool("isWalking", false);
            isWalking = false;
        }

        if (rb.linearVelocityX > 0.01f && isWalking)
        {
            anim.SetFloat("InputX", input.x);
        }
        else if (rb.linearVelocityX < -0.01f && isWalking)
        {
            anim.SetFloat("InputX", input.x);
        }
        else if (rb.linearVelocityY > 0.01f && isWalking)
        {
            anim.SetFloat("InputY", input.y);
        }
        else if (rb.linearVelocityY < -0.01f && isWalking)
        {
            anim.SetFloat("InputY", input.y);
        }

        if (rb.linearVelocityX > 0.01f && !isWalking)
        {
            anim.SetFloat("LastInputX", input.x);
        }
        else if (rb.linearVelocityX < -0.01f && !isWalking)
        {
            anim.SetFloat("LastInputX", input.x);
        }
        else if (rb.linearVelocityY > 0.01f && !isWalking)
        {
            anim.SetFloat("LastInputX", input.y);
        }
        else if (rb.linearVelocityY < -0.01f && !isWalking)
        {
            anim.SetFloat("LastInputX", input.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
        }
    }
}
