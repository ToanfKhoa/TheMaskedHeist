using UnityEngine;

public class FloatingEffectWithoutDOTween : MonoBehaviour
{
    public float floatDistance = 10f; // Distance the object moves up and down
    public float floatSpeed = 2f;      // Speed of the floating motion
    public bool isFloating = true;

    private Vector3 startPos;
    
    private void Start()
    {
        // Record the initial position of the object
        startPos = transform.position;
    }

    private void Update()
    {
        // Calculate the new Y position based on a sine wave
        if( isFloating==true )
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatDistance;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }    
        
    }
}
