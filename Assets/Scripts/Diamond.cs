using UnityEngine;

public class Diamond : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Notify GameManager that the player has found the diamond
            GameManager.Instance.HandlePlayerFound();

            Debug.Log("Diamond collected by player.");

            // Optionally, destroy the diamond after being collected
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.HoldDiamond(this.gameObject);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.HandleDiamondStolen();
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.diamondPickup);
                SoundManager.Instance.PlayMusic(SoundManager.Instance.chaseMusic);
            }
        }
    }
}
