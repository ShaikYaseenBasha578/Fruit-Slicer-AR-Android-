using UnityEngine;

public abstract class BaseSpawner : MonoBehaviour
{
    // Define abstract methods that must be implemented in derived classes
    public abstract void StartSpawning();
    public abstract void StopSpawning();
}
