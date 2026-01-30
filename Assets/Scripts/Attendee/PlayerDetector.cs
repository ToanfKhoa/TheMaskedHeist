using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] GameObject mask;
    [SerializeField] ColorChecker colorChecker;
    [SerializeField] float delayTime = 0.5f;
    [SerializeField] int acceptableColorDifference = 90;
    [SerializeField] UnityEvent onFoundPlayer = new();

    private Coroutine checkPlayerColorCoroutine = null;
    private void Start()
    {
        colorChecker = GetComponent<ColorChecker>();
        if (colorChecker == null)
        {
            Debug.LogError("Player detector needs color checker component to detect player");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController detectedPlayer = collision.GetComponent<PlayerController>();
        if (mask == null || mask.GetComponent<SpriteRenderer>() || detectedPlayer == null) return;
        
        if (checkPlayerColorCoroutine == null)
        {
            checkPlayerColorCoroutine = StartCoroutine(CheckPlayerColorAfterDelay(detectedPlayer.GetMaskColor()));
        }
    }
    IEnumerator CheckPlayerColorAfterDelay(Color playerColor)
    {
        yield return new WaitForSeconds(delayTime);
        if (colorChecker.CompareColorHSV(mask.GetComponent<SpriteRenderer>().color, playerColor) < acceptableColorDifference)
        {
            if (onFoundPlayer != null)
            {
                onFoundPlayer.Invoke();
            }
        }
    }
}
