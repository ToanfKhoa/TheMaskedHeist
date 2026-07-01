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
    [SerializeField] GameObject detectedPlayerAnnouncement;
    //[SerializeField] UnityEvent onFoundPlayer = new();

    PlayerController detectedPlayer = null;
    private Coroutine caughtPlayerAfterDelayCoroutine = null;
    private bool detectionDisabled = false;
    private void Start()
    {
        colorChecker = GetComponent<ColorChecker>();
        if (colorChecker == null){ Debug.LogError("Player detector needs color checker component to detect player"); }
        if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDiamondStolen.AddListener(OnDiamondStolen);
            GameManager.Instance.OnRespawn.AddListener(OnRespawn);
        }
    }

    /// <summary>Sau khi player lấy được kim cương, khách ngừng phát hiện player.</summary>
    private void OnDiamondStolen()
    {
        detectionDisabled = true;
        if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(false);
        if (caughtPlayerAfterDelayCoroutine != null)
        {
            StopCoroutine(caughtPlayerAfterDelayCoroutine);
            caughtPlayerAfterDelayCoroutine = null;
        }
    }

    /// <summary>Khi respawn: khách phát hiện player trở lại (khôi phục trước diamond).</summary>
    private void OnRespawn()
    {
        detectionDisabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
         if (detectionDisabled) return;
         if (GameManager.Instance != null && GameManager.Instance.DiamondStolen) return;
         if (collision.GetComponent<PlayerController>() == null) return;

        detectedPlayer = collision.GetComponent<PlayerController>();

        if (mask == null || mask.GetComponent<SpriteRenderer>() == null || detectedPlayer == null) return;

        Color playerColor = detectedPlayer.GetMaskColor();

        if (colorChecker.CompareColorHSV(mask.GetComponent<SpriteRenderer>().color, playerColor) < acceptableColorDifference)
        {
            if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(true);

            if (caughtPlayerAfterDelayCoroutine == null && (GameManager.Instance == null || !GameManager.Instance.IsGameOver))
            {
                caughtPlayerAfterDelayCoroutine = StartCoroutine(CaughtPlayerAfterDelay());
            }
        }
        else
        {
            if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(false);

            if (caughtPlayerAfterDelayCoroutine != null)
            {
                StopCoroutine(caughtPlayerAfterDelayCoroutine);
                caughtPlayerAfterDelayCoroutine = null;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (detectedPlayer != collision.GetComponent<PlayerController>()) return;

        if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(false);

        if (caughtPlayerAfterDelayCoroutine != null)
        {
            StopCoroutine(caughtPlayerAfterDelayCoroutine);
            caughtPlayerAfterDelayCoroutine = null;
        }
    }
    IEnumerator CaughtPlayerAfterDelay()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.attendeeSusPlayer, 5f);

        yield return new WaitForSeconds(delayTime);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.HandlePlayerFound();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.attendeeFoundPlayer);
        }
        caughtPlayerAfterDelayCoroutine = null;
    }
}
