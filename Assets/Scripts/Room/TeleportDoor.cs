using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    [Header("Settings")]
    public Transform destinationPoint;
    public bool updateCamera = true;

    private bool justArrived = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. If we just arrived here from another door, ignore this trigger!
        if (justArrived) return;

        if (collision.CompareTag("Player") && destinationPoint != null)
        {
            // 2. Check if the destination is actually another Door script
            TeleportDoor targetDoor = destinationPoint.GetComponent<TeleportDoor>();

            // 3. CRITICAL: Tell the target door "The player is coming, disable your trigger momentarily"
            if (targetDoor != null)
            {
                targetDoor.SetJustArrived(true);
            }

            // 4. Teleport the Player
            collision.transform.position = destinationPoint.position;

            // 5. Update Camera
            if (updateCamera)
            {
                RoomBasedCamera cam = Camera.main.GetComponent<RoomBasedCamera>();
                if (cam != null) cam.MoveToNewRoom(destinationPoint.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 6. When the player physically walks OUT of the door trigger, re-enable it
        if (collision.CompareTag("Player"))
        {
            justArrived = false;
        }
    }

    public void SetJustArrived(bool state)
    {
        justArrived = state;
    }
}