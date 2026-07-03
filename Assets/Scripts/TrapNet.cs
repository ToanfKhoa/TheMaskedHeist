using System.Collections;
using UnityEngine;

public class TrapNet : MonoBehaviour
{
    [SerializeField] private Sprite netTrapSprite;
    [SerializeField] private float freezeDuration = 5f;

    private bool _active = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_active) return;
        if (!collision.CompareTag("Player")) return;

        var player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        _active = true;
        StartCoroutine(TrapRoutine(player));
    }

    private IEnumerator TrapRoutine(PlayerController player)
    {
        GameObject net = SpawnNet(player.transform.position);
        player.SetMovementLocked(true);

        yield return new WaitForSeconds(freezeDuration);

        // Player may have been caught mid-trap; only release if still controllable.
        if (GameManager.Instance == null || !GameManager.Instance.IsGameOver)
            player.SetMovementLocked(false);

        if (net != null)
            Destroy(net);

        _active = false;
    }

    private GameObject SpawnNet(Vector3 position)
    {
        if (netTrapSprite == null) return null;

        var netObj = new GameObject("NetTrap");
        netObj.transform.position = position;

        var sr = netObj.AddComponent<SpriteRenderer>();
        sr.sprite = netTrapSprite;
        sr.sortingOrder = 10;

        return netObj;
    }
}
