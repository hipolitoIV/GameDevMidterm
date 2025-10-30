using System.Collections;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    public GameObject cratePrefab;

    // These are set by the GameManager via InitializeLevel()
    private int totalCratesToSpawn;
    private float spawnInterval;
    private int minCrateValue;
    private int maxCrateValue;
    private float minFallSpeed;
    private float maxFallSpeed;

    private int cratesSpawned = 0;
    private float minX, maxX;

    void Start()
    {
        // Calculate screen boundaries
        Vector3 leftEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        minX = leftEdge.x + 10f; // Add 10f buffer
        maxX = rightEdge.x - 10f; // Add 10f buffer
    }

    /// <summary>
    /// Called by the GameManager to set up this spawner based on the current level definition.
    /// </summary>
    public void InitializeLevel(LevelDefinition levelDef)
    {
        totalCratesToSpawn = levelDef.totalCratesToSpawn;
        spawnInterval = levelDef.spawnInterval;
        minCrateValue = levelDef.minCrateValue;
        maxCrateValue = levelDef.maxCrateValue;
        minFallSpeed = levelDef.minFallSpeed;
        maxFallSpeed = levelDef.maxFallSpeed;
    }

    /// <summary>
    /// Called by the GameManager to begin crate spawning for the current level.
    /// </summary>
    public void BeginSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        cratesSpawned = 0;

        if (totalCratesToSpawn <= 0)
        {
            Debug.LogWarning("CrateSpawner: Total crates to spawn is 0 or less. Spawning aborted.");
            yield break;
        }

        while (cratesSpawned < totalCratesToSpawn)
        {
            // Wait for next spawn
            yield return new WaitForSeconds(spawnInterval);

            // Choose random X position within screen bounds
            float spawnX = Random.Range(minX, maxX);
            Vector3 spawnPos = new Vector3(spawnX, transform.position.y, 0);

            // Spawn crate
            GameObject crateGO = Instantiate(cratePrefab, spawnPos, Quaternion.identity);

            // Randomize crate value and fall speed
            int value = Random.Range(minCrateValue, maxCrateValue + 1);
            float speed = Random.Range(minFallSpeed, maxFallSpeed);
            crateGO.GetComponent<CrateController>().Initialize(value, speed);

            cratesSpawned++;
            GameManager.instance.RegisterCrateSpawn();
        }
    }
}
