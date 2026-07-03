using UnityEngine;

public class Obsticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float disableAfterSeconds = 5f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float spinSpeed = 360f; // Degrees per second

    private bool isLaunched = false;
    private Vector3 flyDirection;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Collider2D col;

    void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        col = GetComponent<Collider2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Example usage: Uncomment the line below to test it automatically at start
        // LaunchAndSpin(Vector3.right);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRespawn.AddListener(ResetToInitialState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaunched)
        {
            // 1. Spin in a circle (Rotating around the Z axis)
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

            // 2. Fly away in the calculated direction
            transform.Translate(flyDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }


    public void LaunchAndSpin(int direction)
    {
        flyDirection = new Vector3(direction, 1f, 0f).normalized;

        isLaunched = true;
        col.enabled = false;

        // Disable after 5 seconds
        Invoke(nameof(DisableSelf), disableAfterSeconds);
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
        isLaunched = false; // Reset state in case it gets re-enabled later
    }

    private void ResetToInitialState()
    {
        CancelInvoke(nameof(DisableSelf));

        isLaunched = false;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        col.enabled = true;
        gameObject.SetActive(true);
    }
}