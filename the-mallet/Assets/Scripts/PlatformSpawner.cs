using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlatformSpawner : MonoBehaviour
{
    // Game objects to be spawned
    public GameObject figure;

    // Spawning specifications
    public float spawnRate = 2f;
    public float offset = 2f;

    // Timer 
    private float timer;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > spawnRate)
        {
            Spawn(figure);
            timer = 0;
        }
        timer += Time.deltaTime;
    }

    // Spawns given figure
    private void Spawn(GameObject figure)
    {
        float highOffset = transform.position.y + offset;
        float lowOffset = transform.position.y - offset;

        Instantiate(figure, new Vector3(transform.position.x, Random.Range(highOffset, lowOffset), 0), transform.rotation);
    }

}
