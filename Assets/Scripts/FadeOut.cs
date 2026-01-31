using System.Collections;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public float duration = 1.5f;
    public bool destroyAfterFade = false;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        canvasGroup.alpha = 1f;

        canvasGroup.blocksRaycasts = true;

        StartCoroutine(DoFadeOut());
    }

    private IEnumerator DoFadeOut()
    {
        float timer = 0f;
        float startAlpha = canvasGroup.alpha;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / duration);
            yield return null;
        }


        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;  

        if (destroyAfterFade)
        {
            Destroy(gameObject); 
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }
}
