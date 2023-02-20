using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour
{
    // Defines the type of platform
    public virtual string PlatformType { get { return "BasePlatform"; } }

    // Access to methods from the platform manager class
    public PlatformManager platformManager = PlatformManager.Instance; 


    //*************************************

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }

}
