using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bánh xe mặt nạ (UI). Mặt nạ đang đeo nằm trên đỉnh (nổi trên màn hình),
/// các mặt nạ đã unlock khác trải quanh vòng tròn và khuất xuống dưới.
/// Khi đổi mode (Q/E) bánh xe xoay mượt để đưa mặt nạ mới lên đỉnh,
/// mặt nạ cũ xoay khuất xuống.
///
/// Icon được sinh tự động từ mảng sprite truyền vào Rebuild(), nên không
/// cần tạo tay các object icon trong Editor. Đặt object này dưới một Canvas
/// KHÔNG xoay để "đỉnh = trên màn hình" chuẩn.
/// </summary>
public class MaskWheelUI : MonoBehaviour
{
    [Header("Layout")]
    [Tooltip("Bán kính vòng tròn (đơn vị local). Tăng để đẩy các mặt nạ khác xuống xa/khuất hơn.")]
    [SerializeField] private float radius = 220f;
    [Tooltip("Kích thước mỗi icon mặt nạ (đơn vị local).")]
    [SerializeField] private float iconSize = 120f;
    [Tooltip("Giữ icon luôn thẳng đứng (không nghiêng theo bánh xe).")]
    [SerializeField] private bool keepIconsUpright = true;

    [Header("Animation")]
    [Tooltip("Thời gian xoay mượt giữa 2 mặt nạ (giây).")]
    [SerializeField] private float rotateDuration = 0.25f;

    private readonly List<Image> _icons = new List<Image>();
    private readonly List<int> _slotModes = new List<int>(); // mode index của từng slot theo thứ tự trên vòng
    private float _spacingDeg = 360f;

    private float _currentZ;   // góc xoay đang hiển thị
    private float _targetZ;    // góc xoay đích
    private float _zVel;       // vận tốc dùng cho SmoothDamp
    private int _currentSlot;  // slot đang ở đỉnh

    /// <summary>Dựng lại bánh xe từ danh sách mode đã unlock và bảng sprite theo mode.</summary>
    public void Rebuild(List<int> unlockedModes, Sprite[] modeSprites, int currentMode)
    {
        foreach (var ic in _icons)
            if (ic != null) Destroy(ic.gameObject);
        _icons.Clear();
        _slotModes.Clear();

        int n = unlockedModes != null ? unlockedModes.Count : 0;
        if (n == 0) return;

        _spacingDeg = 360f / n;

        for (int i = 0; i < n; i++)
        {
            int mode = unlockedModes[i];
            _slotModes.Add(mode);

            var go = new GameObject("MaskIcon_" + mode, typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(transform, false);
            rt.sizeDelta = new Vector2(iconSize, iconSize);

            var img = go.GetComponent<Image>();
            img.raycastTarget = false;
            img.preserveAspect = true;
            if (modeSprites != null && mode >= 0 && mode < modeSprites.Length)
                img.sprite = modeSprites[mode];
            img.enabled = img.sprite != null;

            _icons.Add(img);
        }

        LayoutIcons();
        SnapToMode(currentMode);
    }

    /// <summary>Xoay bánh xe để đưa mặt nạ <paramref name="mode"/> lên đỉnh (có animation).</summary>
    public void RotateToMode(int mode)
    {
        int slot = _slotModes.IndexOf(mode);
        if (slot < 0 || _icons.Count == 0) return;

        int step = ShortestSignedStep(_currentSlot, slot, _icons.Count);
        _targetZ += step * _spacingDeg;
        _currentSlot = slot;
    }

    private void LayoutIcons()
    {
        for (int i = 0; i < _icons.Count; i++)
        {
            float ang = i * _spacingDeg * Mathf.Deg2Rad; // 0 = đỉnh, tăng theo chiều kim đồng hồ
            Vector2 pos = new Vector2(Mathf.Sin(ang), Mathf.Cos(ang)) * radius;
            ((RectTransform)_icons[i].transform).anchoredPosition = pos;
        }
    }

    /// <summary>Đặt ngay lập tức (không animation) mặt nạ lên đỉnh.</summary>
    private void SnapToMode(int mode)
    {
        int slot = _slotModes.IndexOf(mode);
        if (slot < 0) slot = 0;

        _currentSlot = slot;
        _targetZ = slot * _spacingDeg;
        _currentZ = _targetZ;
        _zVel = 0f;
        ApplyRotation();
    }

    private void Update()
    {
        if (Mathf.Abs(_currentZ - _targetZ) > 0.01f)
        {
            _currentZ = Mathf.SmoothDamp(_currentZ, _targetZ, ref _zVel, Mathf.Max(0.0001f, rotateDuration));
            ApplyRotation();
        }
    }

    private void ApplyRotation()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, _currentZ);

        if (keepIconsUpright)
        {
            for (int i = 0; i < _icons.Count; i++)
                _icons[i].transform.localEulerAngles = new Vector3(0f, 0f, -_currentZ);
        }
    }

    /// <summary>Số bước có dấu ngắn nhất trên vòng tròn n slot để đi từ a sang b.</summary>
    private static int ShortestSignedStep(int a, int b, int n)
    {
        int d = b - a;
        while (d > n / 2) d -= n;
        while (d < -(n / 2)) d += n;
        return d;
    }
}
