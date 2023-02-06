using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpStrength = 14f;
    public float moveSpeed = 7f;
    public float slamStrength = 25f;
    public float slamBounceMultiplier = 1.3f; 

    private Rigidbody2D rb; // Rigidbody
    private BoxCollider2D coll; // Collider
    private bool onGround; // Stores whether player is on the ground
    private bool usingSlam; // Stores whether slam has been activated
    [SerializeField] private LayerMask jumpableGround; 


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();


    }

    // Update is called once per frame 
    private void Update()
    {
        // Reads and modifies x direction and velocity only if the slam is not being used
        if (!usingSlam)
        {
            float dirX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveSpeed * dirX, rb.velocity.y);
        }
        onGround = IsGrounded();
        // If jump is pressed
        if (Input.GetButtonDown("Jump") && onGround)
        {
            Jump();
        } 
        // If the player is on the air then the slam is activated
        else if (usingSlam || Input.GetButtonDown("Jump") && !onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slamStrength);
            usingSlam = true;
        }

        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Platform tile =  collision.gameObject.GetComponent<Platform>();
        if (tile && usingSlam)  // TODO : usingSlam is always false on collision due to Update() 
        {
            usingSlam = false; // note patch
            tile.SlamHit();
            Bounce();
            // jump ?)
        }
    }

    // Checks if player is touching ground
    private bool IsGrounded() 
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
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
