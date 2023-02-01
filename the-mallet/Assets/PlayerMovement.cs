using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpStrength = 14f;
    public float moveSpeed = 7f;
    public float malletingStrength = 25f;

    private Rigidbody2D rb; // Rigidbody
    private BoxCollider2D coll; // Collider
    private bool onGround; // Stores whether player is on the ground
    private bool usingMallet; // Stores whether mallet has been activated

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
        // Reads and modifies x direction and velocity only if the mallet is not being used
        if (!usingMallet)
        {
            float dirX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveSpeed * dirX, rb.velocity.y);
        }
        
        // Checks if the player is grounded and turns off the mallet if it is
        if (onGround = IsGrounded())
        {
            usingMallet = false;
        }


        // If jump is pressed
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        } 
        // If the player is on the air then the mallet is activated
        else if (usingMallet || Input.GetButtonDown("Jump") && !onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, -malletingStrength);
            usingMallet = true;
        }

        
    }

    // Checks if player is touching ground
    private bool IsGrounded() 
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
