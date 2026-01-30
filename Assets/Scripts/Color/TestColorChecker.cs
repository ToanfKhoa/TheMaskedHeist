using UnityEngine;
using TMPro;

public class TestColorChecker : MonoBehaviour
{
    ColorChecker colorChecker;
    public GameObject target;
    public TextMeshProUGUI scoreText;
    private void Start()
    {
        colorChecker = new ColorChecker();
    }
    private void Update()
    {
        int score = colorChecker.CompareColorHSv(ColorChecker.GetColorFromGameObject(this.gameObject), ColorChecker.GetColorFromGameObject(target));
        scoreText.text = score.ToString();
    }
}
