using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlatformManager : MonoBehaviour
{
    // Implementation of a singleton pattern to ensure that there is only one instance of the PlatformManager 
    public static PlatformManager Instance { get; private set; }

    // Dictionary that maps each platform to its corresponding stack
    private Dictionary<string, Stack<GameObject>> platformStacks = new Dictionary<string, Stack<GameObject>>();

    // Platform prefabs that will belong to the different stacks in the dictionary
    public GameObject movingPlatformPrefab;
    public GameObject normalPlatformPrefab;
    public GameObject basePlatformPrefab;

    // Platform number to be created depending on the platform type
    public int basePlatformNumber;
    public int movingPlatformNumber;
    public int normalPlatformNumber;

    // Default spawn point for platforms
    public GameObject spawnPoint;

    // Spawn Interval
    public float spawnInterval = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        InitializePlatformStacks();
        CreateInitialPlatforms();
        StartCoroutine(SpawnRandomPlatform());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Initializes the corresponding stacks in the dictionary
    private void InitializePlatformStacks()
    {
        platformStacks.Add("MovingPlatform", new Stack<GameObject>());
        platformStacks.Add("NormalPlatform", new Stack<GameObject>());
        platformStacks.Add("BasePlatform", new Stack<GameObject>());
    }

    // Adds the correct number of each type of platform to each stack after creating them. The platforms are set inactive
    private void CreateInitialPlatforms()
    {
        // Base Platforms
        for (int i = 0; i < basePlatformNumber; i++)
        {
            GameObject platform = Instantiate(basePlatformPrefab, new Vector3(i * 2, 0, 0), Quaternion.identity);
            platform.SetActive(false);
            platformStacks["BasePlatform"].Push(platform);
        }

        // Moving Platforms
        for (int i = 0; i < movingPlatformNumber; i++)
        {
            GameObject platform = Instantiate(movingPlatformPrefab, new Vector3(i * 2, 0, 0), Quaternion.identity);
            platform.SetActive(false);
            platformStacks["MovingPlatform"].Push(platform);
        }

        // Normal Platforms
        for (int i = 0; i < normalPlatformNumber; i++)
        {
            GameObject platform = Instantiate(normalPlatformPrefab, new Vector3(i * 2, 0, 0), Quaternion.identity);
            platform.SetActive(false);
            platformStacks["NormalPlatform"].Push(platform);
        }

    }

    // Singleton Pattern. Now This script can be easily referenced by other scripts. It deletes all extra instances of this script on load
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Coroutine that is in charge of spawning random platforms in a given position
    private IEnumerator SpawnRandomPlatform()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Choose a random platform type to spawn
            List<string> platformTypes = new List<string>(platformStacks.Keys);
            string platformType = platformTypes[Random.Range(0, platformTypes.Count)];
            Debug.Log("Spawning a " + platformType);

            // Check if there are any platforms of that type left in the stack
            if (platformStacks[platformType].Count == 0)
            {
                Debug.Log("No more platforms of type " + platformType);
                continue;
            }

            // Get the next platform of the chosen type from the stack
            GameObject platform = platformStacks[platformType].Pop();

            // Set the platform to the spawn point and activate it
            platform.transform.position = spawnPoint.transform.position;
            platform.SetActive(true);
        }
    }


    // Deactivates the platform and returns it to its corresponding stack. It recognises the given platform's type
    public void DeactivatePlatform(GameObject platform)
    {
        // Get the platform's type
        string platformType = platform.GetComponent<BasePlatform>().PlatformType;

        // Check if the platform's type exists in the dictionary
        if (!platformStacks.ContainsKey(platformType))
        {
            Debug.LogError("Error: Platform type " + platformType + " is not defined in the platform stacks.");
            return;
        }

        // Add the platform back to the corresponding stack
        platform.SetActive(false);
        platformStacks[platformType].Push(platform);
    }
}
