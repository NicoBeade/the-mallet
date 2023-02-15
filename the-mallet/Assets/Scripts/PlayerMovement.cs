using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    // Movement constants
    private const float DASH_DISTANCE = 4f;
    private const float MINIMUM_ACCEPTED_DASH_LEFT = -9f;
    private const float MAXIMUM_ACCEPTED_DASH_RIGHT = 10f;

    // Possible actions by player
    enum ACTIONS: int {DASHING = 0, SLAMMING, NO_ACTION, JUMPING, GROUNDED};

    // Public movement magnitudes
    public float jumpStrength;
    public float dashSpeed;
    public float slamStrength;
    public float slamBounceMultiplier; 

    // Unity resources
    private Rigidbody2D rb; // Rigidbody
    private BoxCollider2D coll; // Collider
    
    // State variables
    public int actionInProgress; // Stores action in progress
    private int dashingDirection; // 1: right, -1: left

    // Target position
    private Vector3 targetPosition; // Position to be in after dashing

    // Auxiliary horizontal movement input variables and cooldowns
    private float horizontalMovement; // Stores horizontal movement info
    public float dashCooldown; // Time that has to go by before executing another dash
    private float nextDashTime; // Current time + cooldown
    public float groundCheckWaitTime; // After jumping, the ground check is disabled for a brief window of time
    private float groundEnablingTime; // Current time + grounding wait time


    [SerializeField] private LayerMask jumpableGround; 


    //*************************************

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        actionInProgress = (int)ACTIONS.NO_ACTION;
    }

    // Update is called once per frame 
    private void Update()
    {
        switch (actionInProgress)
        {
            // If the player is on the ground, then the program only admits jumping
            case (int)ACTIONS.GROUNDED:
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                    actionInProgress = (int)ACTIONS.JUMPING;
                }
                break;

            // If the player is on-air then the program accepts dashing and slamming
            case (int)ACTIONS.JUMPING:
                horizontalMovement = Input.GetAxisRaw("Horizontal");
                if (Time.time > nextDashTime && horizontalMovement != 0)
                {
                    if ((horizontalMovement < 0 && rb.position.x > MINIMUM_ACCEPTED_DASH_LEFT) || (horizontalMovement > 0 && rb.position.x < MAXIMUM_ACCEPTED_DASH_RIGHT))
                    {
                        SetupDashDirection((int)Mathf.Sign(horizontalMovement));
                        SetupDashPhysics();
                    }
                }

                // If jumping whilst on-air, the player slams the ground
                else if (Input.GetButtonDown("Jump"))
                {
                    Slam(); // Adopts downwards fast velocity
                    actionInProgress = (int)ACTIONS.SLAMMING;

                }
                break;

            // If the player is dashing the program moves the player
            case (int)ACTIONS.DASHING:
                Dash();
                break;

            // If the player is slamming the player moves downwards fast.
            case (int)ACTIONS.SLAMMING:
                Slam();
                break;

            default:
                break;
        }

        if (Time.time > groundEnablingTime && IsGrounded() && actionInProgress != (int)ACTIONS.DASHING)
        {
            actionInProgress = (int)ACTIONS.GROUNDED;
        }
    }
    
    // Checks if player is touching ground in a downwards direction only
    private bool IsGrounded() 
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .15f, jumpableGround) && !Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, jumpableGround);
    }

    //Checks if player is colliding with an obstacle on the direction its moving.
    private bool SideCollision()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right * dashingDirection, .05f, jumpableGround);
    }

    // Changes the position of the player in the desired direction
    private void Dash()
    {
        // Modifies the horizontal position of the object until the target is reached
        transform.position += Vector3.right * dashSpeed * Time.deltaTime * dashingDirection;  
        if (rb.position.x * dashingDirection >= targetPosition.x * dashingDirection)  
        {
            rb.gravityScale = 3;
            actionInProgress = (int)ACTIONS.JUMPING;
            Debug.Log("Reached Target");
        }

    }

    // Player jump
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        groundEnablingTime = Time.time + groundCheckWaitTime; // Disables ground checks for a brief window of time
    }

    // After destroying a platform the player is sent up the air with a boost
    private void Bounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpStrength * slamBounceMultiplier);
        groundEnablingTime = Time.time + groundCheckWaitTime; // Disables ground checks for a brief window of time
    }

    // Turns off gravity and sets up cooldown
    private void SetupDashPhysics()
    {
        targetPosition += transform.position.z * Vector3.forward + transform.position.y * Vector3.up + transform.position.x * Vector3.right; // Other coordinates remain unchanged
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        nextDashTime = Time.time + dashCooldown;
    }

    // Determines direction of dash and defines target position
    private void SetupDashDirection(int direction)
    {
        dashingDirection = direction;
        targetPosition = Vector3.right * DASH_DISTANCE * dashingDirection; // Adopts targeted position according to direction
        actionInProgress = (int)ACTIONS.DASHING;
    }

    // Slamming mechanic, activated by jumping while on-air
    private void Slam()
    {
        rb.velocity = new Vector2(rb.velocity.x, -slamStrength);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If there is a collision whilst slamming
        if (actionInProgress == (int)ACTIONS.SLAMMING)
        {
            Bounce();
            actionInProgress = (int)ACTIONS.JUMPING;
        }
        else if (actionInProgress == (int)ACTIONS.DASHING)
        {
            dashingDirection *= -1;
            targetPosition -= Vector3.left * DASH_DISTANCE;
        }
    }
}

