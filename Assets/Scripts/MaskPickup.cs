using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [Tooltip("1 = Grayscale, 2 = Cipher")]
    public int modeToUnlock = 1;

    public CUIColorPicker colorPicker;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (GameManager.Instance != null)
            GameManager.Instance.SetCheckpoint(collision.transform.position);

        colorPicker.UnlockMode(modeToUnlock);
        gameObject.SetActive(false);
    }
}
