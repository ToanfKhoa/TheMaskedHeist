using UnityEngine;

public class PatrolMover : MonoBehaviour
{
    [SerializeField] Transform target;
    [Tooltip("Waypoints in this object's local space (Z is ignored for 2D).")]
    [SerializeField] Vector3[] waypoints;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] bool flipOnDirection = false;

    int _nextWaypointIndex;
    float _lastDeltaX;

    void Start()
    {
        if (!IsPathValid())
            return;

        target.position = transform.TransformPoint(waypoints[0]);
        _nextWaypointIndex = 1;
    }

    void Update()
    {
        if (target == null || !IsPathValid())
            return;

        Vector3 currentPos = target.position;
        Vector3 nextPos = transform.TransformPoint(waypoints[_nextWaypointIndex]);
        Vector3 delta = nextPos - currentPos;
        delta.z = 0f;

        float dist = delta.magnitude;
        float step = moveSpeed * Time.deltaTime;

        if (dist <= 0.0001f || step >= dist)
        {
            target.position = nextPos;
            _nextWaypointIndex = (_nextWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            target.position = currentPos + delta.normalized * step;
        }

        if (flipOnDirection && delta.x != 0f)
        {
            float dirX = Mathf.Sign(delta.x);
            if (dirX != _lastDeltaX)
            {
                Vector3 scale = target.localScale;
                scale.x = Mathf.Abs(scale.x) * dirX;
                target.localScale = scale;
                _lastDeltaX = dirX;
            }
        }
    }

    bool IsPathValid()
    {
        return waypoints != null && waypoints.Length >= 2;
    }
}
