using UnityEngine;

public class TestColorChecker : MonoBehaviour
{
    ColorChecker colorChecker;
    public GameObject target;
    private void Start()
    {
        colorChecker = new ColorChecker();
    }
    private void Update()
    {
        int score = colorChecker.CompareColorHSv(ColorChecker.GetColorFromGameObject(this.gameObject), ColorChecker.GetColorFromGameObject(target));
        Debug.Log(score);
    }
}
