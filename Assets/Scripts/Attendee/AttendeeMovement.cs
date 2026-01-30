using UnityEngine;

public class AttendeeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float insideSpeed = 1f;
    [SerializeField] float outsideSpeed = 2f;
    [SerializeField] TargetZone targetZone;
    [SerializeField] Vector2 wanderingInterval = new Vector2(1f, 3f);
    [SerializeField] float minDistanceToTarget = 0.2f;

    private Vector2 wanderingTarget = Vector2.zero;
    private float wanderingTimer = 0f;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogWarning("Attendee needs a Rigidbody2D to move");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetZone == null || rb == null) return;
        Wandering();
    }

    private void Wandering()
    {
        wanderingTimer -= Time.fixedDeltaTime;
        if (wanderingTimer <= 0f)
        {
            wanderingTarget = targetZone.GetRelativeRandomPoint();
            wanderingTimer = Random.Range(wanderingInterval.x, wanderingInterval.y);
        }
        Debug.Log("Relative: " + wanderingTarget);
        Vector2 direction = (wanderingTarget + (Vector2)targetZone.transform.position - (Vector2)transform.position).normalized;
        if ((wanderingTarget + (Vector2)targetZone.transform.position - (Vector2)transform.position).magnitude < minDistanceToTarget)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = direction * (targetZone.IsPointInsideZone(transform.position) ? insideSpeed : outsideSpeed);
    }

    private void OnDrawGizmos()
    {
        
    }
}
