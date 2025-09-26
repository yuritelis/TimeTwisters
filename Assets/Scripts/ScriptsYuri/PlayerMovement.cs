using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 input;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = input * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        anim.SetBool("isWalking", true);

        if (context.canceled)
        {
            anim.SetBool("isWalking", false);
            anim.SetFloat("LastInputX", input.x);
            anim.SetFloat("LastInputY", input.x);
        }
        input = context.ReadValue<Vector2>();
        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
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