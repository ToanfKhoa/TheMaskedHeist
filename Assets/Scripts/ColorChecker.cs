using UnityEngine;

public class ColorChecker : MonoBehaviour
{
    public float CompareColorHSv(Color playerColor, Color attendeeColor)
    {
        float h1, s1, v1;
        float h2, s2, v2;

        // Chuyen doi hai mau tu RGB sang HSV
        Color.RGBToHSV(playerColor, out h1, out s1, out v1);
        Color.RGBToHSV(attendeeColor, out h2, out s2, out v2);

        // Tinh khoang cach Euclidean giua hai mau HSV
        float coichungmaudo = Mathf.Min(Mathf.Abs(h1 - h2), 1 - Mathf.Abs(h1 - h2));
        float distance = Mathf.Sqrt(
            Mathf.Pow(coichungmaudo, 2) * trongso +
            Mathf.Pow(s1 - s2, 2) +
            Mathf.Pow(v1 - v2, 2)
        );

        //chuan hoa ve gia tri tu 0 toi 1
        float colorComparisionValue = 1 - (distance / Mathf.Sqrt(trongso + 2)); //sqrt(3) do khoang cach toi da cua 2 mau la sqrt(3)
        score = (int)(colorComparisionValue * 100);
        //score = Mathf.Round(colorComparisionValue * 100);
        return score;
    }
    private static Color GetColorFromGameObject(GameObject obj)
    {
        // Kiem tra neu object cco spriterenderer
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer.color;
        }

        // Kiem tra neu object co image
        Image image = obj.GetComponent<Image>();
        if (image != null)
        {
            return image.color;
        }

        // Tra ve mau mac dinh neu khong co image va spriterenderer
        return Color.white;
    }
}
