using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MovingPlatform : BasePlatform
{
    // Defines the type of platform
    public override string PlatformType { get { return "MovingPlatform"; } }

    // Movement variables
    public float moveSpeed = 5;
    private const float deadZone = -18;

    //*************************************

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlatform();
        CheckPlatformOutOfBounds();
    }

    // Moves de platform
    public void MovePlatform()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    // Deletes platform
    public void CheckPlatformOutOfBounds()
    {
        if (transform.position.x < deadZone)
        {
            PlatformManager.Instance.DeactivatePlatform(gameObject);
            Debug.Log("Deactivating " + gameObject.name);
        }
    }
}
