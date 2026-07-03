using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý vòng đời bảo vệ theo kim cương:
/// - Khi nhặt kim cương  -> bật hết bảo vệ ở vị trí gốc và cho đuổi.
/// - Khi player bị bắt    -> tắt hết bảo vệ (biến mất).
/// Nhặt lại kim cương sau khi hồi sinh sẽ bật lại bảo vệ từ đầu.
/// Bảo vệ được gán sẵn trong Inspector (đặt sẵn trong scene, không instantiate).
/// </summary>
public class GuardManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> guards = new List<GameObject>();

    private readonly List<Vector3> _originalPositions = new List<Vector3>();

    private void Awake()
    {
        foreach (var g in guards)
            _originalPositions.Add(g != null ? g.transform.position : Vector3.zero);
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDiamondStolen.AddListener(SpawnGuards);
            GameManager.Instance.OnPlayerFound.AddListener(DespawnGuards);
        }
    }

    /// <summary>Bật bảo vệ ở vị trí gốc và cho đuổi player.</summary>
    public void SpawnGuards()
    {
        for (int i = 0; i < guards.Count; i++)
        {
            var g = guards[i];
            if (g == null) continue;

            g.transform.position = _originalPositions[i];
            g.SetActive(true);

            var chase = g.GetComponent<ChasePlayer2D>();
            if (chase != null) chase.enabled = true;
        }
    }

    /// <summary>Tắt hết bảo vệ (khi player bị bắt).</summary>
    public void DespawnGuards()
    {
        foreach (var g in guards)
            if (g != null) g.SetActive(false);
    }
}
