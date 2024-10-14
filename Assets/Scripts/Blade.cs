using Unity.XR.CoreUtils;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;

    private XROrigin xrOrigin; // Reference to the XR Origin
    private Collider sliceCollider;
    private TrailRenderer sliceTrail;

    public Vector3 direction { get; private set; }
    public bool slicing { get; private set; }

    private void Awake()
    {
        xrOrigin = FindObjectOfType<XROrigin>(); // Get XR Origin
        sliceCollider = GetComponent<Collider>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Handle mouse input for testing in the Unity Editor or PC build
        if (Input.GetMouseButtonDown(0))
        {
            StartSlice(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlice();
        }
        else if (slicing)
        {
            ContinueSlice(Input.mousePosition);
        }
#elif UNITY_ANDROID || UNITY_IOS
        // Handle touch input for mobile builds
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                StartSlice(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                StopSlice();
            }
            else if (slicing && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            {
                ContinueSlice(touch.position);
            }
        }
#endif
    }

    private void StartSlice(Vector3 screenPosition)
    {
        // Raycast from the screen position into the AR world
        if (xrOrigin != null)
        {
            Ray ray = xrOrigin.Camera.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                transform.position = hit.point;
            }
        }

        slicing = true;
        sliceCollider.enabled = true;
        sliceTrail.enabled = true;
        sliceTrail.Clear();
    }

    private void StopSlice()
    {
        slicing = false;
        sliceCollider.enabled = false;
        sliceTrail.enabled = false;
    }

    private void ContinueSlice(Vector3 screenPosition)
    {
        // Continue raycasting in the AR space to move the blade
        if (xrOrigin != null)
        {
            Ray ray = xrOrigin.Camera.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 newPosition = hit.point;
                direction = newPosition - transform.position;

                // Calculate the slicing velocity
                float velocity = direction.magnitude / Time.deltaTime;
                sliceCollider.enabled = velocity > minSliceVelocity;

                // Update blade position
                transform.position = newPosition;
            }
        }
    }
}

