using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] float rotaingSpeed = 2.0f;
    [SerializeField] float rotatingRadius = 3.0f;
    [SerializeField] Transform centerPoint;

    private void Start()
    {
        if (centerPoint != null)
        {
            Vector3 direction = (transform.position - centerPoint.position).normalized;
            transform.position = centerPoint.position + direction * rotatingRadius;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (centerPoint == null) return;
        transform.RotateAround(centerPoint.position, Vector3.forward, rotaingSpeed * Time.fixedDeltaTime);
    }
}
