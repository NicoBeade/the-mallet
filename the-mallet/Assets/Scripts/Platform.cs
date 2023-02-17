using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{



    //*************************************

    // Start is called before the first frame update
    void Start()
    {
        //coll = GetComponent<BoxCollider2D>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void SlamHit(GameObject platform )
    {
        DestroyPlatform(platform);
    }

    static public void DestroyPlatform(GameObject platform)
    {
        Destroy(platform);
        Debug.Log("Collided with normal platform");
    }



}
