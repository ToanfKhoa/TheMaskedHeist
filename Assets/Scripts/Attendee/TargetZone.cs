using UnityEngine;

public class TargetZone : MonoBehaviour
{
    public enum ZoneShape { Circle, Rectangle }
    [Header("Target Zone Settings")]
    [SerializeField] ZoneShape zoneShape = ZoneShape.Rectangle;
    [SerializeField] private float radius = 3f;
    [SerializeField] private Vector2 size = new Vector2(3f, 3f);
    [SerializeField] private float deadZoneThinkness = 0.5f;
    [Header("Gizmos Settings")]
    [SerializeField] bool showGizmos = true;
    [SerializeField] Color deadZoneColor = Color.red;
    [SerializeField] Color safeZoneColor = Color.yellow;

    public Vector2 GetRandomPoint()
    {
        switch (zoneShape)
        {
            case ZoneShape.Circle:
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                float randomDistance = Random.Range(0, radius);
                return new Vector2(transform.position.x, transform.position.y) + randomDirection * randomDistance;
            case ZoneShape.Rectangle:
                Vector2 halfSize = size / 2f;
                float randomX = Random.Range(-halfSize.x, halfSize.x);
                float randomY = Random.Range(-halfSize.y, halfSize.y);
                return new Vector2(transform.position.x + randomX, transform.position.y + randomY);
            default:
                return Vector2.zero;
        }
    }

    public Vector2 GetRelativeRandomPoint()
    {
        switch (zoneShape)
        {
            case ZoneShape.Circle:
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                float randomDistance = Random.Range(0, radius);
                return randomDirection * randomDistance;
            case ZoneShape.Rectangle:
                Vector2 halfSize = size / 2f;
                float randomX = Random.Range(-halfSize.x, halfSize.x);
                float randomY = Random.Range(-halfSize.y, halfSize.y);
                return new Vector2(randomX, randomY);
            default:
                return Vector2.zero;
        }
    }

    public bool IsPointInsideZone(Vector2 point)
    {
        Vector2 zonePosition = new Vector2(transform.position.x, transform.position.y);
        switch (zoneShape)
        {
            case ZoneShape.Circle:
                if (deadZoneThinkness >= radius) return false;
                return Vector2.Distance(zonePosition, point) <= radius;
            case ZoneShape.Rectangle:
                if (deadZoneThinkness >= size.x / 2f || deadZoneThinkness >= size.y / 2f) return false;
                Vector2 halfSize = size / 2f;
                return (point.x >= zonePosition.x - halfSize.x + deadZoneThinkness && point.x <= zonePosition.x + halfSize.x - deadZoneThinkness &&
                        point.y >= zonePosition.y - halfSize.y + deadZoneThinkness && point.y <= zonePosition.y + halfSize.y - deadZoneThinkness);
            default:
                return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        switch (zoneShape)
        {
            case ZoneShape.Circle:
                // Draw safe zone
                Gizmos.color = safeZoneColor;
                Gizmos.DrawWireSphere(transform.position, Mathf.Max(0, radius - deadZoneThinkness));
                //Draw dead zone
                Gizmos.color = deadZoneColor;
                Gizmos.DrawWireSphere(transform.position, radius);
                break;
            case ZoneShape.Rectangle:
                // Draw safe zone
                Gizmos.color = safeZoneColor;
                Vector2 deadZoneSize = new Vector2(Mathf.Max(0, size.x - 2 * deadZoneThinkness), Mathf.Max(0, size.y - 2 * deadZoneThinkness));
                Gizmos.DrawWireCube(transform.position, deadZoneSize);
                // Draw dead zone
                Gizmos.color = deadZoneColor;
                Gizmos.DrawWireCube(transform.position, new Vector2(size.x, size.y));
                break;
        }
    }
}
