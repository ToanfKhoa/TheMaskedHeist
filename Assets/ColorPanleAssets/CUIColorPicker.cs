using System;
using UnityEngine;
using UnityEngine.UI;

public class CUIColorPicker : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject filter;
    public GameObject[] passwords;

    public Color Color { get { return _color; } set { Setup( value ); } } 
    public void SetOnValueChangeCallback( Action<Color> onValueChange )
    {
        _onValueChange = onValueChange;
    }
    private Color _color = Color.red;

    //action la kieu du lieu co the chua phuong thuc khong co kieu tra ve, co mot hoac nhieu tham so
    //co the thay doi phuong thuc tro toi khi can thiet ma khong can thay doi ma goi
    //co the dung toan tu += de ket hop nhieu phuong thuc trong mot action
    private Action<Color> _onValueChange;
    private Action _update;

    public void Start()
    {
        passwords = GameObject.FindGameObjectsWithTag( "Password" );
    }

    private static void RGBToHSV( Color color, out float h, out float s, out float v ) //cho biet cac thong so HSV cua mau sac dua vao
    {
        var cmin = Mathf.Min( color.r, color.g, color.b ); //lay so nho nhat
        var cmax = Mathf.Max( color.r, color.g, color.b ); //lay so lon nhat
        var d = cmax - cmin;
        if ( d == 0 ) {
            h = 0;
        } else if ( cmax == color.r ) {
            h = Mathf.Repeat( ( color.g - color.b ) / d, 6 );
        } else if ( cmax == color.g ) {
            h = ( color.b - color.r ) / d + 2;
        } else {
            h = ( color.r - color.g ) / d + 4;
        }
        s = cmax == 0 ? 0 : d / cmax;
        v = cmax;
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
        _update();
    }
}
