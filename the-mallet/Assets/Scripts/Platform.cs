using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D coll; // Collider
    private float moveSpeed;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    void Move()
    {

    }
    public void SlamHit()
    {
        Debug.Log("Slam Hit!!!"); // TODO implement what should 
    }


    


}
