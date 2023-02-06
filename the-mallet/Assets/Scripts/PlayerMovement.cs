using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    // Movement constants
    private const float LANE_DISTANCE = 3.0f;
    private const int LEFT = -1;
    private const int RIGHT = 1;

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
    public int actionInProgress; // Stores action in progress
    private int dashingDirection; // 1: right, -1: left
    private int currentLane = 2; // Lanes are numbered 0-4 from left to right. Default is centered

    // Target position
    private Vector3 targetPosition; // Position to be in after dashing

    // Auxiliary horizontal movement input variables and cooldowns
    private float horizontalMovement; // Stores horizontal movement info
    public float dashCooldown; // Time that has to go by before executing another dash
    private float nextDashTime; // Current time + cooldown

    

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
            case (int)ACTIONS.GROUNDED: // If the player is on the ground, then the program only admits jumping
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                    actionInProgress = (int)ACTIONS.JUMPING;
                }
                break;

            case (int)ACTIONS.JUMPING: // If the player is on-air then the program accepts dashing and slamming
                horizontalMovement = Input.GetAxisRaw("Horizontal");
                if (horizontalMovement != 0 && Time.time > nextDashTime)
                {
                    if(horizontalMovement < 0 && currentLane != 0)
                    {
                        targetPosition = Vector3.left * LANE_DISTANCE; // Adopts targeted position to the next lane to the player's left
                        dashingDirection = LEFT;
                    }
                    else if (horizontalMovement > 0 && currentLane != 4)
                    {
                        targetPosition = Vector3.right * LANE_DISTANCE; // Adopts targeted position to the next lane to the player's left
                        dashingDirection = RIGHT;
                    }
                    targetPosition += transform.position.z * Vector3.forward + transform.position.y * Vector3.up + transform.position.x * Vector3.right; // Other coordinates remain unchanged
                    rb.gravityScale = 0;
                    rb.velocity = Vector2.zero;
                    nextDashTime = Time.time + dashCooldown;
                    
                    actionInProgress = (int)ACTIONS.DASHING;

                }
                else if (Input.GetButtonDown("Jump"))
                {
                    rb.velocity = new Vector2(rb.velocity.x, -slamStrength); // Adopts downwards fast velocity
                    actionInProgress = (int)ACTIONS.SLAMMING;

                }
                break;

            case (int)ACTIONS.DASHING: // If the player is dashing the program moves the player
                MoveLane();
                break;

            case (int)ACTIONS.SLAMMING: // If the player is slamming the player moves downwards fast.
                rb.velocity = new Vector2(rb.velocity.x, -slamStrength);
                break;

            default:
                break;
        }

        actionInProgress = IsGrounded() ? (int)ACTIONS.GROUNDED : actionInProgress;

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
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .005f, jumpableGround);
    }

    // Changes the position of the player to the desired lane.
    private void MoveLane()
    {
        // Checks for invalid movements
        if ((currentLane == 0 && dashingDirection == LEFT) || (currentLane == 4 && dashingDirection == RIGHT))
        {
            rb.gravityScale = 3;
            actionInProgress = (int)ACTIONS.JUMPING;
            return;
        }

        // Modifies the horizontal position of the object until the target is reached
        transform.position += Vector3.right * dashSpeed * Time.deltaTime * dashingDirection;  
        if (Vector3.Distance(transform.position,targetPosition) <= 0.1f) 
        {
            currentLane += dashingDirection;
            rb.gravityScale = 3;
            actionInProgress = (int)ACTIONS.JUMPING;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpStrength);

    }
    private void Bounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpStrength * slamBounceMultiplier);

    }
}
