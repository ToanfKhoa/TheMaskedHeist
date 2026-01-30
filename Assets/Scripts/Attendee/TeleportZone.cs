using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    [SerializeField] TargetZone targetZone;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetZone == null) return;
        collision.transform.position = targetZone.GetRandomPoint();
    }
}
