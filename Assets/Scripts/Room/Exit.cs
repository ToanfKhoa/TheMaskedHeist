using System.Collections;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public float winDelay = 4.0f; 

    private bool hasTriggered = false;

    public CanvasGroup fadePanel;
    public float fadeSpeed = 1.0f;
    public GameObject fadePanelGameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart.AddListener(HideExit);
            GameManager.Instance.OnDiamondStolen.AddListener(ShowExit);
            GameManager.Instance.OnRespawn.AddListener(HideExit);
        }
        HideExit();
    }

    private void HideExit()
    {
        this.gameObject.SetActive(false);
    }

    private void ShowExit()
    {
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);
            if (GameManager.Instance != null)
            {
                StartCoroutine(DelayWin());
                
            }
        }
    }
    private IEnumerator DelayWin()
    {
        hasTriggered = true; 

        Debug.Log("Player reached exit. Waiting for win...");

        if (fadePanel != null)
        {
            StartCoroutine(FadeInPanel());
        }
        StartCoroutine(MoveCarLeft());

        yield return new WaitForSeconds(winDelay);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandleWin();
        }
    }
    private IEnumerator FadeInPanel()
    {
        yield return new WaitForSeconds(3f);
        fadePanelGameObject.SetActive(true);
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadePanel.alpha = alpha;
            yield return null; 
        }
        fadePanel.alpha = 1f;
    }
    private IEnumerator MoveCarLeft()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.carStarting);
        yield return new WaitForSeconds(2f);
        while (true)
        {
            gameObject.transform.Translate(Vector3.left * 10 * Time.deltaTime);
            yield return null;
        }
    }

}
