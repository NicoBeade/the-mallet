using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    // Movement constants
    private const float LANE_DISTANCE = 4f;
    private const int LEFT = -1;
    private const int RIGHT = 1;
    private const float MINIMUM_ACCEPTED_DASH_LEFT = -9f;
    private const float MAXIMUM_ACCEPTED_DASH_RIGHT = 10f;

    // Possible actions by player
    enum ACTIONS: int {DASHING = 0, SLAMMING, NO_ACTION, JUMPING, GROUNDED};

    // Public movement magnitudes
    public float jumpStrength;
    public float dashSpeed;
    public float slamStrength;
    public float slamBounceMultiplier = 1.3f; 
    // Unity resources
    private Rigidbody2D rb; // Rigidbody
    private BoxCollider2D coll; // Collider
    
    // State variables
    private int actionInProgress; // Stores action in progress
    private int dashingDirection; // 1: right, -1: left
    private bool sideCollidedWhenDashing; // Stores if the player collided when dashing


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
                if (Time.time > nextDashTime && horizontalMovement != 0 )
                {
                    if(horizontalMovement < 0 && rb.position.x > MINIMUM_ACCEPTED_DASH_LEFT)
                    {
                        targetPosition = Vector3.left * LANE_DISTANCE; // Adopts targeted position to the next lane to the player's left
                        dashingDirection = LEFT;
                        actionInProgress = (int)ACTIONS.DASHING;
                    }
                    else if (horizontalMovement > 0 && rb.position.x < MAXIMUM_ACCEPTED_DASH_RIGHT)
                    {
                        targetPosition = Vector3.right * LANE_DISTANCE; // Adopts targeted position to the next lane to the player's left
                        dashingDirection = RIGHT;
                        actionInProgress = (int)ACTIONS.DASHING;
                    }

                    // If input indicates to dash, cooldown and target position are and gravity is turned off 
                    if(actionInProgress == (int)ACTIONS.DASHING)
                    {
                        targetPosition += transform.position.z * Vector3.forward + transform.position.y * Vector3.up + transform.position.x * Vector3.right; // Other coordinates remain unchanged
                        rb.gravityScale = 0;
                        rb.velocity = Vector2.zero;
                        nextDashTime = Time.time + dashCooldown;
                    }

                }

                // If jumping when in-air, the player slams the ground
                else if (Input.GetButtonDown("Jump"))
                {
                    rb.velocity = new Vector2(rb.velocity.x, -slamStrength); // Adopts downwards fast velocity
                    actionInProgress = (int)ACTIONS.SLAMMING;

                }
                break;

            // If the player is dashing the program moves the player
            case (int)ACTIONS.DASHING: 
                MoveLane();
                break;

            // If the player is slamming the player moves downwards fast.
            case (int)ACTIONS.SLAMMING: 
                rb.velocity = new Vector2(rb.velocity.x, -slamStrength);
                break;

            default:
                break;
        }

        // Grounding is checked only after a few miliseconds after jumping
        if (Time.time > groundEnablingTime && IsGrounded() && actionInProgress != (int)ACTIONS.DASHING) 
        {
            actionInProgress = (int)ACTIONS.GROUNDED;
        }

    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Platform tile =  collision.gameObject.GetComponent<Platform>();
        if (tile && actionInProgress == (int)ACTIONS.SLAMMING)  // TODO : usingSlam is always false on collision due to Update() 
        {
            actionInProgress = (int)ACTIONS.JUMPING;
            tile.SlamHit();
            Bounce();
            // jump ?)
        }
    }

    // Checks if player is touching ground
    private bool IsGrounded() 
    {
        // Checks if the player is toughing the ground but only in downwards direction
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround) && !Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, jumpableGround);
    }

    //Checks if player is colliding with an obstacle on the direction its moving.
    private bool SideCollision()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right * dashingDirection, .05f, jumpableGround);
    }

    // Changes the position of the player to the desired lane.
    private void MoveLane()
    {
        // Modifies the horizontal position of the object until the target is reached
        transform.position += Vector3.right * dashSpeed * Time.deltaTime * dashingDirection;  
        if (rb.position.x * dashingDirection >= targetPosition.x * dashingDirection)  
        {
            rb.gravityScale = 3;
            actionInProgress = (int)ACTIONS.JUMPING;
            sideCollidedWhenDashing = false; // Resets auxiliary variable 
        }

        //Checks for collisions when dashing and returns to the original position if it collides
        if (!sideCollidedWhenDashing && SideCollision())
        {
            dashingDirection *= -1;
            targetPosition -= Vector3.left * LANE_DISTANCE * dashingDirection;
            sideCollidedWhenDashing = true;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        groundEnablingTime = Time.time + groundCheckWaitTime; // Disables ground checks for a brief window of time

    }
    private void Bounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpStrength * slamBounceMultiplier);

    }



}
