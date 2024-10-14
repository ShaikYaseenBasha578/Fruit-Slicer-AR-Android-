using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
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
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            GameObject prefab = GetPooledObject();

            if (Random.value < bombChance)
            {
                prefab = GetPooledBomb();
            }

            if (prefab != null)
            {
                // Set spawn position below the camera and in front of the user
                Vector3 position = new Vector3
                {
                    x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                    y = Camera.main.transform.position.y - 3f,  // Adjust the y value to control how far below the fruits spawn
                    z = Camera.main.transform.position.z + 1f    // Set slightly in front of the camera
                };

                Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

                prefab.transform.position = position;
                prefab.transform.rotation = rotation;
                prefab.SetActive(true);

                // Apply upward force relative to the camera's up direction
                Rigidbody rb = prefab.GetComponent<Rigidbody>();
                if (rb != null)  // Ensure rigidbody exists
                {
                    rb.velocity = Vector3.zero; // Reset velocity before applying force
                    float force = Random.Range(minForce, maxForce);
                    rb.AddForce(Camera.main.transform.up * force, ForceMode.Impulse);
                }

                // Disable after lifetime
                StartCoroutine(DisableAfterLifetime(prefab, maxLifetime));
            }

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
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
