using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CUIColorPicker : MonoBehaviour
{
    public GameObject playerObject;

    // Cố định Saturation và Value ở mức bình thường
    private const float FIXED_SATURATION = 1f;
    private const float FIXED_VALUE = 1f;

    public Color Color { get { return _color; } set { Setup(value); } }

    public void SetOnValueChangeCallback(Action<Color> onValueChange)
    {
        _onValueChange = onValueChange;
    }

    // ── Chế độ ──────────────────────────────────────────────
    private bool _isGrayscaleMode = false;

    private void Start()
    {
        //test
        //SetGrayscaleMode(true);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                Debug.Log("Bạn đang chạm trúng: " + result.gameObject.name);
            }
        }

        _update();
    }
    /// <summary>Bật / tắt chế độ đen trắng từ bên ngoài (ví dụ: nút Toggle).</summary>
    public void SetGrayscaleMode(bool enabled)
    {
        _isGrayscaleMode = enabled;
        Setup(_color);          // vẽ lại UI theo chế độ mới
    }

    // ── Trạng thái nội bộ ───────────────────────────────────
    private Color _color = Color.red;
    private Action<Color> _onValueChange;
    private Action _update;

    // ── Chuyển đổi màu ──────────────────────────────────────
    /// <summary>Chuyển HSV → Color (giống Color.HSVToRGB nhưng tường minh hơn).</summary>
    private static Color HSVToRGB(float h, float s, float v)
    {
        if (s == 0f) return new Color(v, v, v);

        h = Mathf.Repeat(h, 6f);
        int i = (int)h;
        float f = h - i;
        float p = v * (1 - s);
        float q = v * (1 - s * f);
        float t = v * (1 - s * (1 - f));

        switch (i)
        {
            case 0: return new Color(v, t, p);
            case 1: return new Color(q, v, p);
            case 2: return new Color(p, v, t);
            case 3: return new Color(p, q, v);
            case 4: return new Color(t, p, v);
            default: return new Color(v, p, q);
        }
    }

    // ── Helpers UI ──────────────────────────────────────────
    private static bool GetLocalMouse(GameObject go, out Vector2 result)
    {
        var rt = (RectTransform)go.transform;
        var mp = rt.InverseTransformPoint(Input.mousePosition);
        result.x = Mathf.Clamp(mp.x, rt.rect.min.x, rt.rect.max.x);
        result.y = Mathf.Clamp(mp.y, rt.rect.min.y, rt.rect.max.y);
        return rt.rect.Contains(mp);
    }

    private static Vector2 GetWidgetSize(GameObject go)
    {
        return ((RectTransform)go.transform).rect.size;
    }

    private GameObject GO(string name) => transform.Find(name).gameObject;

    // ── Tạo Sprite gradient 1D (dọc) ────────────────────────
    /// <summary>Tạo texture gradient dọc từ <paramref name="bottom"/> đến <paramref name="top"/>.</summary>
    private static Sprite MakeGradientSprite(Color bottom, Color top, int height = 64)
    {
        var tex = new Texture2D(1, height) { wrapMode = TextureWrapMode.Clamp };
        for (int y = 0; y < height; y++)
            tex.SetPixel(0, y, Color.Lerp(bottom, top, y / (float)(height - 1)));
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, height), new Vector2(0.5f, 0.5f));
    }

    /// <summary>Tạo texture hue spectrum (6 đoạn màu cơ bản).</summary>
    private static Sprite MakeHueSprite(int height = 128)
    {
        var tex = new Texture2D(1, height) { wrapMode = TextureWrapMode.Clamp };
        for (int y = 0; y < height; y++)
        {
            float hue = y / (float)(height - 1) * 6f;
            tex.SetPixel(0, y, HSVToRGB(hue, FIXED_SATURATION, FIXED_VALUE));
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, height), new Vector2(0.5f, 0.5f));
    }

    // ── Setup chính ─────────────────────────────────────────
    private void Setup(Color inputColor)
    {
        var sliderGO = GO("Hue");          // Thanh điều chỉnh duy nhất (dùng lại tên cũ)
        var sliderKnob = GO("Hue/Knob");
        var sliderSz = GetWidgetSize(sliderGO);

        if (_isGrayscaleMode)
            SetupGrayscale(sliderGO, sliderKnob, sliderSz, inputColor);
        else
            SetupHue(sliderGO, sliderKnob, sliderSz, inputColor);
    }

    // ── Chế độ màu sắc (Hue) ────────────────────────────────
    private void SetupHue(GameObject sliderGO, GameObject sliderKnob,
                            Vector2 sliderSz, Color inputColor)
    {
        sliderGO.GetComponent<Image>().sprite = MakeHueSprite();

        // Tính Hue từ màu đầu vào
        float h, s, v;
        Color.RGBToHSV(inputColor, out h, out s, out v);
        float Hue = h * 6f;   // [0, 6)

        // Áp dụng màu với Saturation/Value cố định
        Action applyColor = () =>
        {
            var result = HSVToRGB(Hue, FIXED_SATURATION, FIXED_VALUE);
            ApplyResult(result);
        };

        applyColor();
        sliderKnob.transform.localPosition = new Vector2(
            sliderKnob.transform.localPosition.x,
            Hue / 6f * sliderSz.y);

        // Kéo thanh Hue
        Action dragH = null;
        Action idle = () =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mp;
                if (GetLocalMouse(sliderGO, out mp))
                    _update = dragH;
            }
        };

        dragH = () =>
        {
            Vector2 mp;
            GetLocalMouse(sliderGO, out mp);
            Hue = mp.y / sliderSz.y * 6f;
            applyColor();
            sliderKnob.transform.localPosition =
                new Vector2(sliderKnob.transform.localPosition.x, mp.y);
            if (Input.GetMouseButtonUp(0))
                _update = idle;
        };

        _update = idle;
    }

    // ── Chế độ đen trắng (Grayscale) ────────────────────────
    private void SetupGrayscale(GameObject sliderGO, GameObject sliderKnob,
                                  Vector2 sliderSz, Color inputColor)
    {
        sliderGO.GetComponent<Image>().sprite = MakeGradientSprite(Color.black, Color.white);

        // Lấy giá trị xám từ màu đầu vào
        float Gray = inputColor.grayscale;

        Action applyColor = () =>
        {
            var result = new Color(Gray, Gray, Gray);
            ApplyResult(result);
        };

        applyColor();
        sliderKnob.transform.localPosition = new Vector2(
            sliderKnob.transform.localPosition.x,
            Gray * sliderSz.y);

        Action dragG = null;
        Action idle = () =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mp;
                if (GetLocalMouse(sliderGO, out mp))
                    _update = dragG;
            }
        };

        dragG = () =>
        {
            Vector2 mp;
            GetLocalMouse(sliderGO, out mp);
            Gray = Mathf.Clamp01(mp.y / sliderSz.y);
            applyColor();
            sliderKnob.transform.localPosition =
                new Vector2(sliderKnob.transform.localPosition.x, mp.y);
            if (Input.GetMouseButtonUp(0))
                _update = idle;
        };

        _update = idle;
    }

    // ── Cập nhật kết quả ────────────────────────────────────
    private void ApplyResult(Color result)
    {
        playerObject.GetComponent<SpriteRenderer>().color = result;
        if (_color != result)
        {
            _onValueChange?.Invoke(result);
            _color = result;
        }
    }

    void Awake() => Color = Color.green;
}