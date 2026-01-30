using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour
{
    [Header("Wall Settings")]
    [Tooltip("Shrinks Left and Right walls inward")]
    [SerializeField] private float insetX = 0f;

    [Tooltip("Shrinks ONLY the Top wall downward (Good for wall thickness)")]
    [SerializeField] private float insetTop = 0f;

    [Tooltip("Shrinks ONLY the Bottom wall upward")]
    [SerializeField] private float insetBottom = 0f;

    private RoomBasedCamera cam;
    private EdgeCollider2D wallCollider;

    private void Awake()
    {
        BoxCollider2D roomZone = GetComponent<BoxCollider2D>();

        if (wallCollider == null)
        {
            wallCollider = gameObject.AddComponent<EdgeCollider2D>();
        }

        CreateWalls(roomZone);
    }

    private void Start()
    {
        cam = Camera.main.GetComponent<RoomBasedCamera>();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && wallCollider != null)
        {
            CreateWalls(GetComponent<BoxCollider2D>());
        }
    }

    private void CreateWalls(BoxCollider2D zone)
    {
        float halfWidth = zone.size.x / 2f;
        float halfHeight = zone.size.y / 2f;
        Vector2 offset = zone.offset;

        // Calculate specific positions for each side
        // We add Inset to Min values, and subtract Inset from Max values
        float leftX = -halfWidth + insetX;
        float rightX = halfWidth - insetX;
        float bottomY = -halfHeight + insetBottom; // Set this to 0 in Inspector to keep bottom regular
        float topY = halfHeight - insetTop;

        // Define the 4 corners manually using these calculated coordinates
        List<Vector2> points = new List<Vector2>
        {
            new Vector2(leftX, bottomY) + offset,  // Bottom Left
            new Vector2(rightX, bottomY) + offset, // Bottom Right
            new Vector2(rightX, topY) + offset,    // Top Right
            new Vector2(leftX, topY) + offset,     // Top Left
            new Vector2(leftX, bottomY) + offset   // Close Loop
        };

        wallCollider.SetPoints(points);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (cam != null) cam.MoveToNewRoom(transform.position);
        }
    }
}