using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    // Cached reference to the camera
    private RoomBasedCamera cam;

    private void Start()
    {
        // Find the camera script automatically
        cam = Camera.main.GetComponent<RoomBasedCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering is the Player
        if (collision.CompareTag("Player"))
        {
            // Tell the camera to move to THIS object's position
            cam.MoveToNewRoom(transform.position);
        }
    }
}