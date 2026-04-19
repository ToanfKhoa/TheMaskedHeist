using UnityEngine;

public class DarkRoom : MonoBehaviour
{
    [SerializeField] SpriteMask visionMask;
    [Tooltip("Waypoints in this object's local space (Z is ignored for 2D).")]
    [SerializeField] Vector3[] localPath;
    [SerializeField] float moveSpeed = 2f;

    int _nextWaypointIndex;

    void Start()
    {
        if (!IsPathValid())
            return;

        visionMask.transform.position = transform.TransformPoint(localPath[0]);
        _nextWaypointIndex = 1;
    }

    void Update()
    {
        if (visionMask == null || !IsPathValid())
            return;

        Vector3 pos = visionMask.transform.position;
        Vector3 target = transform.TransformPoint(localPath[_nextWaypointIndex]);
        Vector3 delta = target - pos;
        delta.z = 0f;

        float dist = delta.magnitude;
        float step = moveSpeed * Time.deltaTime;

        if (dist <= 0.0001f || step >= dist)
        {
            visionMask.transform.position = target;
            _nextWaypointIndex = (_nextWaypointIndex + 1) % localPath.Length;
        }
        else
        {
            visionMask.transform.position = pos + delta.normalized * step;
        }
    }

    bool IsPathValid()
    {
        return localPath != null && localPath.Length >= 2;
    }
}
