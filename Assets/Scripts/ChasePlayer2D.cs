using UnityEngine;

public class ChasePlayer2D : MonoBehaviour
{
    public float speed = 3f;
    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        Vector3 currentScale = transform.localScale;

        if (player.position.x > transform.position.x)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
        }
        else if (player.position.x < transform.position.x)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }

        transform.localScale = currentScale;
    }
}