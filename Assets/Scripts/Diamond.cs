using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Tooltip("Điểm hồi sinh sau khi nhặt kim cương (đặt cách xa viên kim cương để phần khôi phục có ý nghĩa). Bỏ trống = dùng ngay vị trí kim cương.")]
    [SerializeField] private Transform respawnPoint;

    private Vector3 _originalPosition;
    private Transform _originalParent;
    private Vector3 _checkpointPos;
    private bool _collected = false;

    private void Awake()
    {
        _originalPosition = transform.position;
        _originalParent = transform.parent;
        _checkpointPos = respawnPoint != null ? respawnPoint.position : transform.position;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerFound.AddListener(ResetDiamond);
    }

    // Dùng Stay để vẫn nhặt lại được khi player hồi sinh ngay trên viên kim cương.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_collected) return;
        if (!collision.CompareTag("Player")) return;

        _collected = true;
        Debug.Log("Diamond collected by player.");

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.HoldDiamond(this.gameObject);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCheckpoint(_checkpointPos);
            GameManager.Instance.HandleDiamondStolen();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.diamondPickup);
            SoundManager.Instance.PlayMusic(SoundManager.Instance.chaseMusic);
        }
    }

    /// <summary>Khi player bị bắt: đưa kim cương về chỗ cũ để có thể nhặt lại.</summary>
    private void ResetDiamond()
    {
        _collected = false;
        transform.SetParent(_originalParent);
        transform.position = _originalPosition;
        gameObject.SetActive(true);
    }
}
