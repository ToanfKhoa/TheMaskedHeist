using UnityEngine;
using System; // Required for Action

public class RoomBasedCamera : MonoBehaviour
{
    [Header("Settings")]
    public float smoothSpeed = 5f;
    public float arrivalThreshold = 0.01f; // Distance at which we consider "arrived"

    // Actions (Signals)
    public Action OnTransitionStarted;
    public Action OnTransitionReached;

    private Vector3 targetPosition;
    private bool isTransitioning = false;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (isTransitioning)
        {
            // Move the camera
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // Check if we are close enough to the target
            if (Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
            {
                transform.position = targetPosition; // Snap to exact position
                isTransitioning = false;

                // Fire the "Reached" signal
                OnTransitionReached?.Invoke();
            }
        }
    }

    public void MoveToNewRoom(Vector3 newRoomPosition)
    {
        targetPosition = new Vector3(newRoomPosition.x, newRoomPosition.y, transform.position.z);

        // Only trigger the start signal if we aren't already at that position
        if (transform.position != targetPosition)
        {
            isTransitioning = true;

            // Fire the "Started" signal
            OnTransitionStarted?.Invoke();
        }
    }
}