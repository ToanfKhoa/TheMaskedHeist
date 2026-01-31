using UnityEngine;

public class RoomBasedCamera : MonoBehaviour
{
    [Header("Settings")]
    public float smoothSpeed = 5f; // How fast the camera pans

    // The target position the camera is trying to reach
    private Vector3 targetPosition;
    float yOffset = 2f;

    private void Start()
    {
        // Start at the current camera position
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        // If the camera is not at the target, move towards it smoothly
        if (transform.position != targetPosition)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.position = newPos;
        }
    }


    // This function will be called by the Room objects
    public void MoveToNewRoom(Vector3 newRoomPosition)
    {
        // Keep the camera's Z position (usually -10) so we don't clip through the background
        targetPosition = new Vector3(newRoomPosition.x, newRoomPosition.y + yOffset, transform.position.z);
    }
}