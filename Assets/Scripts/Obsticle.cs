using UnityEngine;

public class Obsticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float spinSpeed = 360f; // Degrees per second

    private bool isLaunched = false;
    private Vector3 flyDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Example usage: Uncomment the line below to test it automatically at start
        // LaunchAndSpin(Vector3.right); 
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

        // Disable after 5 seconds
        Invoke(nameof(DisableSelf), 5f);
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
        isLaunched = false; // Reset state in case it gets re-enabled later
    }
}