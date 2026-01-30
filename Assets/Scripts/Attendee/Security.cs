using UnityEngine;

public class Security : MonoBehaviour
{
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
