using UnityEngine;
using System.Collections;

public class DogDetector : MonoBehaviour
{
    [SerializeField] float delayTime = 0.5f;
    [SerializeField] float resumePatrolDelay = 1f;
    [SerializeField] GameObject detectedPlayerAnnouncement;
    [SerializeField] PatrolMover patrolMover;
    [SerializeField] Animator animator;
    [SerializeField] string patrolingAnimatorParam = "IsPatroling";

    PlayerController detectedPlayer = null;
    private Coroutine caughtPlayerAfterDelayCoroutine = null;
    private Coroutine resumePatrolCoroutine = null;
    private bool detectionDisabled = false;

    private void Start()
    {
        if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDiamondStolen.AddListener(OnDiamondStolen);
            GameManager.Instance.OnRespawn.AddListener(OnRespawn);
        }
    }

    /// <summary>Sau khi player lấy được kim cương, dog ngừng phát hiện player.</summary>
    private void OnDiamondStolen()
    {
        detectionDisabled = true;
        StopDetection();
    }

    /// <summary>Khi respawn: dog phát hiện player trở lại (khôi phục trước diamond).</summary>
    private void OnRespawn()
    {
        detectionDisabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (detectionDisabled) return;
        if (GameManager.Instance != null && GameManager.Instance.DiamondStolen) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        detectedPlayer = player;

        if (resumePatrolCoroutine != null)
        {
            StopCoroutine(resumePatrolCoroutine);
            resumePatrolCoroutine = null;
        }
        SetChasing(true);
        if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(true);

        if (caughtPlayerAfterDelayCoroutine == null && (GameManager.Instance == null || !GameManager.Instance.IsGameOver))
        {
            caughtPlayerAfterDelayCoroutine = StartCoroutine(CaughtPlayerAfterDelay());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (detectedPlayer != collision.GetComponent<PlayerController>()) return;

        detectedPlayer = null;
        if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(false);

        if (caughtPlayerAfterDelayCoroutine != null)
        {
            StopCoroutine(caughtPlayerAfterDelayCoroutine);
            caughtPlayerAfterDelayCoroutine = null;
        }

        if (resumePatrolCoroutine == null)
        {
            resumePatrolCoroutine = StartCoroutine(ResumePatrolAfterDelay());
        }
    }

    private void StopDetection()
    {
        if (resumePatrolCoroutine != null)
        {
            StopCoroutine(resumePatrolCoroutine);
            resumePatrolCoroutine = null;
        }
        SetChasing(false);
        if (detectedPlayerAnnouncement != null) detectedPlayerAnnouncement.SetActive(false);

        if (caughtPlayerAfterDelayCoroutine != null)
        {
            StopCoroutine(caughtPlayerAfterDelayCoroutine);
            caughtPlayerAfterDelayCoroutine = null;
        }
    }

    IEnumerator ResumePatrolAfterDelay()
    {
        yield return new WaitForSeconds(resumePatrolDelay);
        SetChasing(false);
        resumePatrolCoroutine = null;
    }

    private void SetChasing(bool isChasing)
    {
        if (patrolMover != null) patrolMover.SetChasing(isChasing);
        if (animator != null && !string.IsNullOrEmpty(patrolingAnimatorParam))
        {
            animator.SetBool(patrolingAnimatorParam, !isChasing);
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
