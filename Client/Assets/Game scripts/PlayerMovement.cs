using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;


    private Animator animator;
    private float dirx = 0f;
    private SpriteRenderer sprite;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    [SerializeField] private LayerMask jumpableGround;


    private enum MovmentState {idle, running,jumping,falling }


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirx = Input.GetAxisRaw("Horizontal");
        rb.velocity=new Vector2(dirx* moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump")&& IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        UpdateAnimationState();

    }
    private void UpdateAnimationState()
    {
        MovmentState state;
       
        if (dirx > 0f)
        {
            state = MovmentState.running;
            sprite.flipX = false;

        }
        else if (dirx < 0f)
        {
            sprite.flipX = true;
            state = MovmentState.running;
        }
        else
        {
            state = MovmentState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state |= MovmentState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state |= MovmentState.falling;
        }
        animator.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center,coll.bounds.size,0f,Vector2.down,.1f, jumpableGround); 
    }

}
