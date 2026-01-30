using UnityEngine;

public class Exit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart.AddListener(HideExit);
            GameManager.Instance.OnDiamondStolen.AddListener(ShowExit);
        }
    }

    private void HideExit()
    {
        this.gameObject.SetActive(false);
    }

    private void ShowExit()
    {
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("VICTORY");
        }
    }
}
