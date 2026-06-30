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

    public GameObject filter;
    public GameObject[] passwords;

    // ── Chế độ ──────────────────────────────────────────────
    public enum MaskMode
    {
        Normal,
        Grayscale,
        Cipher,
        TrapMask
    }
    private MaskMode currentMode = MaskMode.Normal;
    private readonly List<int> _unlockedModes = new List<int> { 0 }; // thứ tự unlock quyết định thứ tự cycle
    private GameObject[] _trapObjects;

    // ── Nút đổi mode trên UI (vd. nút GRAY) + sprite mặt nạ tương ứng ──
    [Header("Mode Button UI")]
    [SerializeField] private Image modeButtonIcon; // Image của nút bấm, sẽ đổi sprite theo mode
    [SerializeField] private Sprite[] modeSprites = new Sprite[4]; // theo thứ tự enum MaskMode: Normal, Grayscale, Cipher, TrapMask

    public void SetOnValueChangeCallback(Action<Color> onValueChange)
    {
        _onValueChange = onValueChange;
    }


    //action la kieu du lieu co the chua phuong thuc khong co kieu tra ve, co mot hoac nhieu tham so
    //co the thay doi phuong thuc tro toi khi can thiet ma khong can thay doi ma goi
    //co the dung toan tu += de ket hop nhieu phuong thuc trong mot action
    private Action<Color> _onValueChange;
    private Action _update;

    public void Start()
    {
        passwords = GameObject.FindGameObjectsWithTag("Password");
        _trapObjects = GameObject.FindGameObjectsWithTag("Trap");
        SetTrapSpritesVisible(false);
    }

    private static bool GetLocalMouse(GameObject go, out Vector2 result) //kiem tra tro chuot co trong object ko, tra ve vi tri cuc bo
    {
        var rt = (RectTransform)go.transform;
        var mp = rt.InverseTransformPoint(Input.mousePosition);
        result.x = Mathf.Clamp(mp.x, rt.rect.min.x, rt.rect.max.x);
        result.y = Mathf.Clamp(mp.y, rt.rect.min.y, rt.rect.max.y);
        return rt.rect.Contains(mp);
    }

    private static Vector2 GetWidgetSize(GameObject go) //tra ve kich thuoc cua gameobject
    {
        var rt = (RectTransform)go.transform;
        return rt.rect.size;
    }

    private GameObject GO(string name) //tim va tra ve mot object con co ten mong muon
    {
        return transform.Find(name).gameObject;
    }

    public Color SetRandomColor() //random mau
    {
        var rng = new System.Random(); //tao so ngau nhien
        var r = (rng.Next() % 1000) / 1000.0f;    //next de lay mot so ngau nhien tu the hien cua lop random
        var g = (rng.Next() % 1000) / 1000.0f;
        var b = (rng.Next() % 1000) / 1000.0f;
        Color Color = new Color(r, g, b);   //tao mau ngau nhien cho color
        return Color;
    }

    void Awake()
    {
        Color = Color.green; //mac dinh ban dau color la green
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
        HandleModeSwitch();
        _update();
    }

    private void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.E))
            CycleMode(1);
        else if (Input.GetKeyDown(KeyCode.Q))
            CycleMode(-1);
    }

    /// <summary>Chuyển sang mode kế tiếp/trước trong danh sách đã unlock.</summary>
    private void CycleMode(int direction)
    {
        if (_unlockedModes.Count <= 1) return;

        int idx = _unlockedModes.IndexOf((int)currentMode);
        SetMaskMode(_unlockedModes[(idx + direction + _unlockedModes.Count) % _unlockedModes.Count]);
    }

    /// <summary>Gắn vào OnClick của nút UI (vd. nút GRAY) để cycle mode giống phím E.</summary>
    public void OnModeButtonClicked()
    {
        CycleMode(1);
    }

    public void UnlockMode(int modeIndex)
    {
        if (modeIndex >= 0 && modeIndex < 4 && !_unlockedModes.Contains(modeIndex))
            _unlockedModes.Add(modeIndex);
    }

    public void SetMaskMode(int i)
    {
        bool wasTrapMask = currentMode == MaskMode.TrapMask;

        switch (i)
        {
            case 0: currentMode = MaskMode.Normal; break;
            case 1: currentMode = MaskMode.Grayscale; break;
            case 2: currentMode = MaskMode.Cipher; break;
            case 3: currentMode = MaskMode.TrapMask; break;
        }

        if (currentMode == MaskMode.TrapMask)
        {
            filter.SetActive(true);
            filter.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
            SetTrapSpritesVisible(true);
        }
        else
        {
            if (wasTrapMask) SetTrapSpritesVisible(false);
            filter.SetActive(currentMode == MaskMode.Cipher);
        }

        UpdateModeSprite();
        Setup(_color);
    }

    /// <summary>Đổi sprite icon trên nút UI và sprite mặt nạ player đang đeo theo mode hiện tại.</summary>
    private void UpdateModeSprite()
    {
        int idx = (int)currentMode;
        if (modeSprites == null || idx < 0 || idx >= modeSprites.Length) return;

        Sprite sprite = modeSprites[idx];
        if (sprite == null) return;

        if (modeButtonIcon != null)
            modeButtonIcon.sprite = sprite;

        if (playerObject != null)
        {
            var sr = playerObject.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = sprite;
        }
    }

    private void SetTrapSpritesVisible(bool visible)
    {
        foreach (var trap in _trapObjects)
        {
            var sr = trap.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = visible;
        }
    }

    // ── Trạng thái nội bộ ───────────────────────────────────
    private Color _color = Color.red;

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

        if (currentMode == MaskMode.Grayscale)
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
        // Cipher và TrapMask giữ màu gốc sprite, không tô màu theo người chơi
        if (currentMode == MaskMode.Cipher || currentMode == MaskMode.TrapMask)
            playerObject.GetComponent<SpriteRenderer>().color = Color.white;
        else
            playerObject.GetComponent<SpriteRenderer>().color = result;

        if (currentMode == MaskMode.Cipher)
        {
            //doi mau cho cac nen cua password
            foreach (var p in passwords)
            {
                var pImg = p.GetComponent<SpriteRenderer>(); //lay image cua tung password
                if (pImg != null)
                    pImg.color = result; //doi mau cho tung password
            }

            // doi mau filter
            var resImgFilter = filter.GetComponent<SpriteRenderer>();
            Color newColor = result;
            newColor.a = 0.2f;
            if (resImgFilter != null)
                resImgFilter.color = newColor;
        }

        if (_color != result)
        {
            _onValueChange?.Invoke(result);
            _color = result;
        }
    }
}