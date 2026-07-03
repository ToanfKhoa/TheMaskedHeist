using System;
using UnityEngine;

public class PlayerDetectorBaby : MonoBehaviour
{
    public Action<GameObject> OnPlayerDetected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController playerController))
        {
            OnPlayerDetected?.Invoke(collision.gameObject);
        }
    }
}
