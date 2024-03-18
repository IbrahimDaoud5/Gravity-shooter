using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Cinemachine;

public class PlayerMovementMulti : NetworkBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    Transform childTransform;

    private Animator animator;
    private float dirx = 0f;
    private SpriteRenderer sprite;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private CinemachineVirtualCamera vc;
    [SerializeField] private AudioListener listener;

    [SerializeField] private LayerMask jumpableGround;


    private enum MovmentState { idle, running, jumping, falling }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            vc.Priority = 1;
        }
        else
        {
            vc.Priority = 0;
        }

    }
    private void Start()
    {

        childTransform = transform.Find("bow");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

    }



    // Update is called once per frame
    private void Update()
    {


        if (!IsOwner) return;

        if (PauseMenu.gameIsPaused == false)
        {
            dirx = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(dirx * moveSpeed, rb.velocity.y);


            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            UpdateAnimationState();
        }

    }
    private void UpdateAnimationState()
    {
        MovmentState state;

        if (dirx > 0f)
        {
            state = MovmentState.running;
            sprite.flipX = false;
            UpdatePlayerFlipServerRpc(false);
            UpdatePlayerFlipClientRpc(false);

            // Assuming the character is facing right, adjust bow position
            if (childTransform != null)
            {
                Vector3 childPosition = childTransform.localPosition;
                childPosition.x = Mathf.Abs(childPosition.x); // Ensure the x position is positive
                childTransform.localPosition = childPosition;
            }
        }
        else if (dirx < 0f)
        {
            state = MovmentState.running;
            sprite.flipX = true;
            UpdatePlayerFlipServerRpc(true);
            UpdatePlayerFlipClientRpc(true);

            // Assuming the character is facing left, adjust bow position
            if (childTransform != null)
            {
                Vector3 childPosition = childTransform.localPosition;
                childPosition.x = -Mathf.Abs(childPosition.x); // Ensure the x position is negative
                childTransform.localPosition = childPosition;
            }
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
    [ServerRpc]
    void UpdatePlayerFlipServerRpc(bool flipValue)
    {
        UpdatePlayerFlipClientRpc(flipValue);
    }

    [ClientRpc]
    void UpdatePlayerFlipClientRpc(bool flipValue)
    {
        if (sprite != null)
        {
            sprite.flipX = flipValue;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

}