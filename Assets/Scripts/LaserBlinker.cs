using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LaserBlinker : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float onDuration = 2f;
    [SerializeField] private float offDuration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool startOn = true;

    [Header("Collision")]
    [SerializeField] private Collider2D laserCollider;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (laserCollider == null)
            laserCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        StartCoroutine(BlinkLoop());
    }

    private IEnumerator BlinkLoop()
    {
        SetAlpha(startOn ? 1f : 0f);
        SetColliderEnabled(startOn);
        if (!startOn)
            yield return new WaitForSeconds(offDuration);

        while (true)
        {
            SetColliderEnabled(true);
            yield return FadeTo(1f, fadeDuration);
            yield return new WaitForSeconds(onDuration);

            yield return FadeTo(0f, fadeDuration);
            SetColliderEnabled(false);
            yield return new WaitForSeconds(offDuration);
        }
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = spriteRenderer.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, timer / duration));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    private void SetColliderEnabled(bool isEnabled)
    {
        if (laserCollider != null)
            laserCollider.enabled = isEnabled;
    }
}
