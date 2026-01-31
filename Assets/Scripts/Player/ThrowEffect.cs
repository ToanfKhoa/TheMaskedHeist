using UnityEngine;

public class ThrowEffect : MonoBehaviour
{
    [Header("(Motion Settings)")]
    [Tooltip("Tốc độ bay từ phải sang trái")]
    public float throwForceX = 5f;

    [Tooltip("Lực ném lên/xuống ban đầu. Số dương là ném vồng lên, số âm là ném cắm xuống")]
    public float initialThrowForceY = 2f;

    [Tooltip("Độ nặng của vật, càng cao vật rơi càng nhanh")]
    public float gravity = 9.8f;

    [Header("Cấu hình Xoay (Rotation Settings)")]
    [Tooltip("Tốc độ xoay vòng của vật")]
    public float rotationSpeed = 360f;

    private Vector3 currentVelocity;

    void Start()
    {
        currentVelocity = new Vector3(-throwForceX, initialThrowForceY, 0);
    }

    void Update()
    {
        currentVelocity += Vector3.down * gravity * Time.deltaTime;

        transform.position += currentVelocity * Time.deltaTime;
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}
