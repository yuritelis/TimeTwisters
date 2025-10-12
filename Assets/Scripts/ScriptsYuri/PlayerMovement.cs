using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float knockbackDecay = 12f; // controla o quanto o knockback desacelera
    private Rigidbody2D rb;
    private Vector2 input;
    private Animator anim;
    public Player_Combat player_Combat;
    private bool isKnockedBack;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (TimelineUI.isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!isKnockedBack)
        {
            rb.linearVelocity = input * moveSpeed;

            if (Input.GetButtonDown("Slash"))
                player_Combat.Attack();
        }
        else
        {
            // desacelera apenas enquanto o knockback ainda está ativo
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, knockbackDecay * Time.deltaTime);
        }
    }


    public void Move(InputAction.CallbackContext context)
    {
        if (TimelineUI.isPaused || isKnockedBack)
        {
            anim.SetBool("isWalking", false);
            input = Vector2.zero;
            return;
        }

        anim.SetBool("isWalking", !context.canceled);

        if (context.canceled)
        {
            anim.SetFloat("LastInputX", input.x);
            anim.SetFloat("LastInputY", input.y);
            input = Vector2.zero;
        }
        else
        {
            input = context.ReadValue<Vector2>();
        }

        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }


    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    /*public void Sprint(InputAction.CallbackContext context)
    {
        anim.SetBool("isRunning", true);

        if (context.canceled)
        {
            anim.SetBool("isRunning", false);
            anim.SetFloat("LastInputX", input.x);
            anim.SetFloat("LastInputY", input.x);
        }
        input = context.ReadValue<Vector2>();
        anim.SetFloat("InputX", input.x * 2);
        anim.SetFloat("InputY", input.y * 2);
    }*/
}