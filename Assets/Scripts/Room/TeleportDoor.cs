using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    [Header("Settings")]
    public Transform destinationPoint; // Made public for the Editor script
    public bool updateCamera = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && destinationPoint != null)
        {
            collision.transform.position = destinationPoint.position;

            if (updateCamera)
            {
                RoomBasedCamera cam = Camera.main.GetComponent<RoomBasedCamera>();
                if (cam != null) cam.MoveToNewRoom(destinationPoint.position);
            }
        }
    }

    private void UpdateCameraLocation()
    {
        // Tries to find the RoomBasedCamera we made earlier
        RoomBasedCamera cam = Camera.main.GetComponent<RoomBasedCamera>();
        if (cam != null)
        {
            // Tell camera to focus on the destination immediately
            cam.MoveToNewRoom(destinationPoint.position);
        }
    }
}