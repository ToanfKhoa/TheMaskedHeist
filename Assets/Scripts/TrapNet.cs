using System.Collections;
using UnityEngine;

public class TrapNet : MonoBehaviour
{
    [SerializeField] private Sprite netTrapSprite;
    [SerializeField] private float catchAnimDuration = 1f;

    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered) return;
        if (!collision.CompareTag("Player")) return;

        _triggered = true;
        var player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        SpawnNet(player.transform.position);
        if (GameManager.Instance != null)
            GameManager.Instance.HandlePlayerFound();
    }

    private void SpawnNet(Vector3 position)
    {
        if (netTrapSprite == null) return;

        var netObj = new GameObject("NetTrap");
        netObj.transform.position = position;

        var sr = netObj.AddComponent<SpriteRenderer>();
        sr.sprite = netTrapSprite;
        sr.sortingOrder = 10;
    }
}
