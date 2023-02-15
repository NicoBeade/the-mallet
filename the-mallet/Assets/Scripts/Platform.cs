using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Unity resources
    private BoxCollider2D coll; 

    // Public movement magnitudes
    public float moveSpeed;


    //*************************************

    // Start is called before the first frame update
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
        Debug.Log("Slam Hit!!!"); 
    }


    


}
