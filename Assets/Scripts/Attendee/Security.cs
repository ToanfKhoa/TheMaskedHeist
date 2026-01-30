using UnityEngine;

public class Security : MonoBehaviour
{
    private ChasePlayer2D chasePlayer2D;

    private void Start()
    {
        chasePlayer2D = GetComponent<ChasePlayer2D>();
        chasePlayer2D.enabled = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDiamondStolen.AddListener(StartFollow);
        }
    }

    void StartFollow()
    {
        chasePlayer2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.HandlePlayerFound();
            }
        }

        if (collision.CompareTag("Obsticle"))
        {
            Obsticle obsticle = collision.GetComponent<Obsticle>();
            if (obsticle != null)
            {
                obsticle.LaunchAndSpin((int)transform.localScale.x);
            }
        }
    }
}
