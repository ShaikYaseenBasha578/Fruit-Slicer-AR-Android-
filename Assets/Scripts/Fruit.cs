using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject whole;   // The whole fruit object
    public GameObject sliced;  // The sliced fruit object

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private ParticleSystem juiceEffect;

    public int points = 1;

    private void Awake()
    {
        // Get the components needed for slicing and physics
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        // Reset the state of the fruit when it's respawned
        fruitCollider.enabled = true;      // Enable the collider for slicing
        whole.SetActive(true);             // Show the whole fruit
        sliced.SetActive(false);           // Hide the sliced fruit

        // Reset velocity and angular velocity of the fruit
        fruitRigidbody.velocity = Vector3.zero;
        fruitRigidbody.angularVelocity = Vector3.zero;

        // Optionally, reset position/rotation if needed:
        // transform.position = new Vector3(...);  // Reset to the original position
        // transform.rotation = Quaternion.identity;
    }

    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        // Increase the score when the fruit is sliced
        GameManager.Instance.IncreaseScore(points);

        // Disable the whole fruit and collider after slicing
        fruitCollider.enabled = false;
        whole.SetActive(false);

        // Activate the sliced fruit and play the juice particle effect
        sliced.SetActive(true);
        juiceEffect.Play();

        // Rotate the sliced fruit based on the direction of the slice
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Apply force to each slice
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody slice in slices)
        {
            slice.velocity = fruitRigidbody.velocity; // Inherit velocity from the whole fruit
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }

        // Return the fruit to the pool after a short delay
        Invoke(nameof(ReturnToPool), 2f); // Delay to allow sliced animation and effects
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the blade's direction and slice force, then call the Slice method
            Blade blade = other.GetComponent<Blade>();
            if (blade != null)
            {
                Slice(blade.direction, blade.transform.position, blade.sliceForce);
            }
        }
    }

    private void ReturnToPool()
    {
        // Return the fruit to the object pool by deactivating it
        gameObject.SetActive(false);
    }
}

