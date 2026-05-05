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
    public GameObject filter;
    public GameObject[] passwords;

    public Color Color { get { return _color; } set { Setup( value ); } } 
    public void SetOnValueChangeCallback( Action<Color> onValueChange )
    {
        _onValueChange = onValueChange;
    }

    // ── Chế độ ──────────────────────────────────────────────
    private bool _isGrayscaleMode = false;
    //action la kieu du lieu co the chua phuong thuc khong co kieu tra ve, co mot hoac nhieu tham so
    //co the thay doi phuong thuc tro toi khi can thiet ma khong can thay doi ma goi
    //co the dung toan tu += de ket hop nhieu phuong thuc trong mot action
    private Action<Color> _onValueChange;
    private Action _update;

    public void Start()
    {
        passwords = GameObject.FindGameObjectsWithTag( "Password" );
    }

    private void Start()
    {
        //test
        //SetGrayscaleMode(true);
    }

    private static bool GetLocalMouse( GameObject go, out Vector2 result ) //kiem tra tro chuot co trong object ko, tra ve vi tri cuc bo
    {
        var rt = ( RectTransform )go.transform;
        var mp = rt.InverseTransformPoint( Input.mousePosition );
        result.x = Mathf.Clamp( mp.x, rt.rect.min.x, rt.rect.max.x );
        result.y = Mathf.Clamp( mp.y, rt.rect.min.y, rt.rect.max.y );
        return rt.rect.Contains( mp );
    }

    private static Vector2 GetWidgetSize( GameObject go ) //tra ve kich thuoc cua gameobject
    {
        var rt = ( RectTransform )go.transform;
        return rt.rect.size;
    }

    private GameObject GO( string name ) //tim va tra ve mot object con co ten mong muon
    {
        return transform.Find( name ).gameObject;
    }

    private void Setup( Color inputColor )
    {
        var satvalGO = GO( "SaturationValue" ); //chua doi tuong dai dien cho phan tu UI de dieu chinh do bao hoa, mau sac
        var satvalKnob = GO( "SaturationValue/Knob" ); //chua gameobject dai dien cho num dieu khien do bao hoa (saturation)
        var hueGO = GO( "Hue" ); //chua doi tuong dai dien cho phan tu UI de dieu chinh mau sac (hue)
        var hueKnob = GO( "Hue/Knob" ); //num dieu khien mau sac
        var result = playerObject; //chua doi tuong result dai dien cho noi hien thi mau sac
        var hueColors = new Color [] { //tao mang chua 6 mau co ban
            Color.red,
            Color.yellow,
            Color.green,
            Color.cyan, //xanh lam nhat
            Color.blue,
            Color.magenta, //mau hong
        };
        var satvalColors = new Color [] { //mang chua cac mau do bao hoa
            new Color( 0, 0, 0 ),  //mau den
            new Color( 0, 0, 0 ),  //mau den
            new Color( 1, 1, 1 ),  //mau trang
            hueColors[0], //mau chi so 0 cua mang hueColors
        };

        var hueTex = new Texture2D( 1, 7 ); //tao thanh hien thi mau sac
        for ( int i = 0; i < 7; i++ ) { //thiet lap mau sac cho tung pixel cua thanh mau vua tao
            hueTex.SetPixel( 0, i, hueColors[i % 6] ); //chi so thu nhat la cot(do chi 1 pixel rong nen cot 0), chi so i la hang, lay mau tu mang hueColors
        }
        hueTex.Apply(); //cap nhat thay doi cho texture (viec cap nhat co the ton nhieu tai nguyen trong Unity nen can dung apply de cap nhat thay doi mot lan)
        hueGO.GetComponent<Image>().sprite = Sprite.Create( hueTex, new Rect( 0, 0.5f, 1, 6 ), new Vector2( 0.5f, 0.5f ) ); //tao sprite tu huetex va gan no cho huego, thanh mau thuc su duoc hien thi
        var hueSz = GetWidgetSize( hueGO ); //gan kich thuoc cua huego-thanh mau cho huesz

        var satvalTex = new Texture2D(2,2); //tao texture2d co kich thuoc rong 2 pixel, dai 2 pixel
        satvalGO.GetComponent<Image>().sprite = Sprite.Create( satvalTex, new Rect( 0.5f, 0.5f, 1, 1 ), new Vector2( 0.5f, 0.5f ) ); //tao sprite tu satvaltex moi tao va gan no cho sprite cua satvalgo
        Action resetSatValTexture = () => {
            for ( int j = 0; j < 2; j++ ) {
                for ( int i = 0; i < 2; i++ ) {
                    satvalTex.SetPixel( i, j, satvalColors[i + j * 2] ); //thiet lap mau cho tung pixel cua satvaltex voi moi mau trong satvalcolors
                }
            }
            satvalTex.Apply(); //cap nhat thay doi
        };
        var satvalSz = GetWidgetSize( satvalGO ); //lay size cua satvalgo gan vao satvalsz

        float Hue, Saturation, Value;
        RGBToHSV( inputColor, out Hue, out Saturation, out Value ); //chuyen mau inputcolor thanh hsv ghi vao cac bien tuong ung

        Action applyHue = () => {
            var i0 = Mathf.Clamp( ( int )Hue, 0, 5 ); //lay gia tri tu Hue(da ep ve int) trong gioi han voi gia tri max la 5, min la 0
            var i1 = ( i0 + 1 ) % 6; //cong 1 de co thu tu so du co the co la 1,2,3,4,5,0
            var resultColor = Color.Lerp( hueColors[i0], hueColors[i1], Hue - i0 ); //noi suy gia tri, tham so thu ba quyet dinh ket qua se nghieng bao nhieu ve tham so thu nhat hay thu hai, neu 0 thi ket qua la tham so thu nhat, neu 1 thi ket qua la tham so thu hai
                                                                                    //hue-i0 la de lay sai khac phan thap phan
            satvalColors[3] = resultColor; //gan mau sac chi so 3 cua satvalcolors bang resultcolor
            resetSatValTexture();  //cap nhat lai mau sac cho satvaltexture
        };
        Action applySaturationValue = () => {
            var sv = new Vector2( Saturation, Value ); //tao vector voi do bao hoa, gia tri sang 
            var isv = new Vector2( 1 - sv.x, 1 - sv.y ); //tao vector nghich dao voi vector sv, tao ra ti le nguoc lai cua s,v , dung de anh can doi anh huong cua mau nen
            //tinh cac toan mau trung gian
            var c0 = isv.x * isv.y * satvalColors[0];   //anh huong goc toi (mau den)- bao hoa thap, sang thap
            var c1 = sv.x * isv.y * satvalColors[1];    //bao hoa cao, sang thap
            var c2 = isv.x * sv.y * satvalColors[2];    //bao hoa thap, sang cao
            var c3 = sv.x * sv.y * satvalColors[3];     //bao hoa cao, sang cao
            var resultColor = c0 + c1 + c2 + c3;    //tinh tong

            var resImg = result.GetComponent<SpriteRenderer>(); //lay image cua result-doi tuong dai dien cho noi hien thi mau sac
            if(resImg != null)
                resImg.color = resultColor;     //doi mau cho image

            //doi mau cho cac nen cua password
            foreach (var p in passwords)
            {
                var pImg = p.GetComponent<SpriteRenderer>(); //lay image cua tung password
                if(pImg != null)
                    pImg.color = resultColor; //doi mau cho tung password
            }

            // doi mau filter
            var resImgFilter = filter.GetComponent<SpriteRenderer>();
            Color newColor = resultColor;
            newColor.a = 0.2f;
            if(resImgFilter != null)
                resImgFilter.color = newColor;

            if ( _color != resultColor ) {  //neu mau co thay doi, kich hoat callback
                if ( _onValueChange != null ) {
                    _onValueChange( resultColor );
                }
                _color = resultColor;
            }
        };

        applyHue();
        applySaturationValue();

        satvalKnob.transform.localPosition = new Vector2( Saturation * satvalSz.x, Value * satvalSz.y ); //gan vi tri cua satvalknob dua vao rong, dai cua satvalsize va saturation, value (saturation, value di tu 0 toi 1)
        hueKnob.transform.localPosition = new Vector2( hueKnob.transform.localPosition.x, Hue / 6 * satvalSz.y ); //gan vi tri cua hueknob dua tren hue voi 6 mau va chieu dai cua satvalsz
        
        Action dragH = null; //khoi tao ham callback chuc nang xu li viec keo hueknob
        Action dragSV = null;//khoi tao ham callback chuc nang xu li viec keo satvalknob

        Action idle = () => {
            if ( Input.GetMouseButtonDown( 0 ) ) { //thuc hien ham khi phat hien chuot duoc nhan xuong
                Vector2 mp;
                if ( GetLocalMouse( hueGO, out mp ) ) { //co nhan vao huego hay khong
                    _update = dragH;    //kich hoat dragHz
                } else if ( GetLocalMouse( satvalGO, out mp ) ) { //co nhan vao satvalgo khong
                    _update = dragSV;   //kich hoat dragSV
                }
            }
        };

        dragH = () => {
            Vector2 mp;
            GetLocalMouse( hueGO, out mp ); //lay vi tri cuc bo cua chuot tu huego
            Hue = mp.y / hueSz.y * 6;       //lay ti le mp.y va huesz.y (chieu doc) de chuan hoa thanh so tu 0 toi 1, sau do nhan 6 de lay gia tri tu 0 toi 5
            applyHue(); //cap nhat lai hue
            applySaturationValue(); //cap nhat lai saturation, value
            hueKnob.transform.localPosition = new Vector2( hueKnob.transform.localPosition.x, mp.y ); //cap nhat vi tri hueknob theo truc y
            if ( Input.GetMouseButtonUp( 0 ) ) {    //lay ti le mp.y va huesz.y (chieu doc) de chuan hoa thanh so tu 0 toi 1
                _update = idle;
            }
        };
        dragSV = () => {
            Vector2 mp;
            GetLocalMouse( satvalGO, out mp );  //lay vi tri cuc bo cua chuot tu satvalgo
            Saturation = mp.x / satvalSz.x;     //lay ti le mp.x va satval.x (chieu ngang) de chuan hoa thanh so tu 0 toi 1
            Value = mp.y / satvalSz.y;          //lay ti le mp.y va huesz.y (chieu doc) de chuan hoa thanh so tu 0 toi 1
            //applySaturationValue(); //cap nhat lai saturation,value
            satvalKnob.transform.localPosition = mp; //cap nhat vi tri cua satvalknob
            if ( Input.GetMouseButtonUp( 0 ) ) {    //lay ti le mp.y va huesz.y (chieu doc) de chuan hoa thanh so tu 0 toi 1
                _update = idle;
            }
        };

        _update = idle; //ve trang thai khong hoat dong idle
    }

    public Color SetRandomColor() //random mau
    {
        var rng = new System.Random(); //tao so ngau nhien
        var r = ( rng.Next() % 1000 ) / 1000.0f;    //next de lay mot so ngau nhien tu the hien cua lop random
        var g = ( rng.Next() % 1000 ) / 1000.0f;
        var b = ( rng.Next() % 1000 ) / 1000.0f;
        Color Color = new Color( r, g, b );   //tao mau ngau nhien cho color
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
    void Update() => _update();
}