using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : BaseSpawner
{
    public override void StartSpawning()
    {
        enabled = true; // Enable spawning
    }

    public override void StopSpawning()
    {
        enabled = false; // Disable spawning
    }

    private Collider spawnArea;
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)]
    public float bombChance = 0.05f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;

    public int poolSize = 20;

    private List<GameObject> objectPool;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        spawnArea = GetComponent<Collider>();

        // Initialize object pool
        objectPool = new List<GameObject>();

        // Pre-instantiate fruits and bombs into the pool
        for (int i = 0; i < poolSize; i++)
        {
            foreach (GameObject fruitPrefab in fruitPrefabs)
            {
                GameObject fruit = Instantiate(fruitPrefab);
                fruit.SetActive(false);
                objectPool.Add(fruit);
            }

            GameObject bomb = Instantiate(bombPrefab);
            bomb.SetActive(false);
            objectPool.Add(bomb);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);  // Initial delay before spawning starts

        while (enabled)
        {
            GameObject prefab = GetPooledObject();  // Get a fruit from the pool

            if (Random.value < bombChance)
            {
                prefab = GetPooledBomb();  // Choose a bomb based on chance
            }

            if (prefab != null)
            {
                // Set spawn position below the camera, within a range in the X direction
                Vector3 spawnPosition = new Vector3
                {
                    x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),  // Randomize the X position
                    y = spawnArea.bounds.min.y,  // Use the spawner's Y position
                    z = spawnArea.bounds.min.z   // Use the spawner's Z position
                };

                // Random rotation for some variation
                Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

                prefab.transform.position = spawnPosition;
                prefab.transform.rotation = rotation;
                prefab.SetActive(true);

                // Apply an upward force relative to the camera's up direction
                Rigidbody rb = prefab.GetComponent<Rigidbody>();
                if (rb != null)  // Ensure the rigidbody exists
                {
                    rb.velocity = Vector3.zero;  // Reset velocity before applying force
                    float force = Random.Range(minForce, maxForce);  // Randomize force
                    Vector3 launchDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1, 0);  // Slight X variation, no Z movement
                    rb.AddForce(launchDirection * force, ForceMode.Impulse);  // Apply the force to launch upwards
                }

                // Disable the object after a certain lifetime
                StartCoroutine(DisableAfterLifetime(prefab, maxLifetime));
            }

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));  // Delay before next spawn
        }
    }

    private GameObject GetPooledObject()
    {
        foreach (GameObject obj in objectPool)
        {
            // Check for null or inactive objects
            if (obj != null && !obj.activeInHierarchy && obj.CompareTag("Fruit"))
            {
                return obj;
            }
        }

        // If no available fruit, instantiate a new one
        return InstantiateFruit();
    }

    private GameObject GetPooledBomb()
    {
        foreach (GameObject obj in objectPool)
        {
            // Check for null or inactive objects
            if (obj != null && !obj.activeInHierarchy && obj.CompareTag("Bomb"))
            {
                return obj;
            }
        }

        // If no available bomb, instantiate a new one
        return InstantiateBomb();
    }

    private GameObject InstantiateFruit()
    {
        GameObject fruit = Instantiate(fruitPrefabs[Random.Range(0, fruitPrefabs.Length)]);
        fruit.SetActive(false);
        objectPool.Add(fruit);
        return fruit;
    }

    private GameObject InstantiateBomb()
    {
        GameObject bomb = Instantiate(bombPrefab);
        bomb.SetActive(false);
        objectPool.Add(bomb);
        return bomb;
    }

    private IEnumerator DisableAfterLifetime(GameObject obj, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (obj != null)  // Ensure object hasn't been destroyed
        {
            obj.SetActive(false);
        }
    }


}
