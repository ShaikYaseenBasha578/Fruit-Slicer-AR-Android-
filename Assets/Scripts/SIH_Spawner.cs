using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SIH_Spawner : BaseSpawner
{
    private Collider spawnArea;
    public GameObject[] fruitPrefabs;
    public GameObject syringePrefab;
    public GameObject pillBoxPrefab;
    public GameObject medicineBottlePrefab;

    [Range(0f, 1f)]
    public float specialObjectChance = 0.15f; // Combined chance for all special objects

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
        InitializeObjectPool();
    }

    private void InitializeObjectPool()
    {
        // Pre-instantiate objects into the pool
        AddObjectsToPool(fruitPrefabs);
        AddToPool(syringePrefab);
        AddToPool(pillBoxPrefab);
        AddToPool(medicineBottlePrefab);
    }

    private void AddObjectsToPool(GameObject[] prefabs)
    {
        foreach (GameObject prefab in prefabs)
        {
            for (int i = 0; i < poolSize / prefabs.Length; i++) // Distribute evenly
            {
                AddToPool(prefab);
            }
        }
    }

    private void AddToPool(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    public override void StartSpawning()
    {
        enabled = true;
        StartCoroutine(Spawn());
    }

    public override void StopSpawning()
    {
        enabled = false;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f); // Initial delay

        while (enabled)
        {
            GameObject prefab = GetPooledObject(); // Default to a fruit

            if (Random.value < specialObjectChance)
            {
                prefab = GetRandomSpecialObject(); // Choose special object based on chance
            }

            if (prefab != null)
            {
                SpawnObject(prefab);
            }

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay)); // Delay before next spawn
        }
    }

    private void SpawnObject(GameObject prefab)
    {
        Vector3 spawnPosition = new Vector3
        {
            x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            y = spawnArea.bounds.min.y,
            z = spawnArea.bounds.min.z
        };

        Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
        prefab.transform.position = spawnPosition;
        prefab.transform.rotation = rotation;
        prefab.SetActive(true);

        Rigidbody rb = prefab.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            float force = Random.Range(minForce, maxForce);
            Vector3 launchDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1, 0);
            rb.AddForce(launchDirection * force, ForceMode.Impulse);
        }

        StartCoroutine(DisableAfterLifetime(prefab, maxLifetime));
    }

    private GameObject GetPooledObject()
    {
        foreach (GameObject obj in objectPool)
        {
            if (obj != null && !obj.activeInHierarchy && obj.CompareTag("Fruit"))
            {
                return obj;
            }
        }

        return null; // Return null if no fruit is available
    }

    private GameObject GetRandomSpecialObject()
    {
        List<GameObject> specialObjects = new List<GameObject>
        {
            syringePrefab,
            pillBoxPrefab,
            medicineBottlePrefab
        };

        // Remove null entries from the list
        specialObjects.RemoveAll(obj => obj == null);

        if (specialObjects.Count > 0)
        {
            string tag = specialObjects[Random.Range(0, specialObjects.Count)].tag;
            return GetPooledObjectByTag(tag) ?? InstantiateSpecialObject(tag);
        }

        return null; // Return null if no special objects are available
    }

    private GameObject GetPooledObjectByTag(string tag)
    {
        foreach (GameObject obj in objectPool)
        {
            if (obj != null && !obj.activeInHierarchy && obj.CompareTag(tag))
            {
                return obj;
            }
        }

        return null;
    }

    private GameObject InstantiateSpecialObject(string tag)
    {
        GameObject prefab = tag switch
        {
            "Syringe" => syringePrefab,
            "PillBox" => pillBoxPrefab,
            "MedicineBottle" => medicineBottlePrefab,
            _ => null
        };

        if (prefab != null)
        {
            GameObject specialObject = Instantiate(prefab);
            specialObject.SetActive(false);
            objectPool.Add(specialObject);
            return specialObject;
        }

        return null;
    }

    private IEnumerator DisableAfterLifetime(GameObject obj, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (obj != null)
        {
            obj.SetActive(false);
        }
    }

    // Reset object pool when restarting the game
    public void ResetSpawner()
    {
        foreach (var obj in objectPool)
        {
            obj.SetActive(false);
        }
        InitializeObjectPool();
    }
}
