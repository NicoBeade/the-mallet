using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovingPlatform : Platform
{
    // Movement variables
    public float moveSpeed = 5;
    private float deadZone = -18;


    //*************************************

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CleanScreen();
    }

    // Moves de platform
    public void Move()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    // Deletes platform
    public void CleanScreen()
    {
        if (transform.position.x < deadZone)
        {
            Platform.DestroyPlatform(gameObject);
        }
    }
}
