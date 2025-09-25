using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    float moveSpeed = 1f;

    bool isFacingRight = true;

    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (isFacingRight)
        {

        }

        FlipSprite();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        anim.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void FlipSprite()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
        }
    }
}