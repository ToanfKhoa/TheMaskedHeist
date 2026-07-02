using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Tooltip("Điểm hồi sinh sau khi nhặt kim cương (đặt cách xa viên kim cương để phần khôi phục có ý nghĩa). Bỏ trống = dùng ngay vị trí kim cương.")]
    [SerializeField] private Transform respawnPoint;
    [Tooltip("Sorting order của sprite kim cương khi đang cầm (để vẽ trên mặt nạ player).")]
    [SerializeField] private int heldSortingOrder = 100;

    private Vector3 _originalPosition;
    private Transform _originalParent;
    private Vector3 _checkpointPos;
    private SpriteRenderer _skin;
    private int _originalSortingOrder;
    private bool _collected = false;

    private void Awake()
    {
        _originalPosition = transform.position;
        _originalParent = transform.parent;
        _checkpointPos = respawnPoint != null ? respawnPoint.position : transform.position;
        _skin = GetComponentInChildren<SpriteRenderer>();
        if (_skin != null) _originalSortingOrder = _skin.sortingOrder;
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
            if (_skin != null) _skin.sortingOrder = heldSortingOrder; // vẽ trên mặt nạ player
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
        if (_skin != null) _skin.sortingOrder = _originalSortingOrder;
        gameObject.SetActive(true);
    }
}
