using System.Collections;
using UnityEngine;

public class BabyAttendee : MonoBehaviour
{
    [SerializeField] private float _playTime = 4f;
    [SerializeField] private float _coolDownTime = 10f;
    [SerializeField] private PlayerDetectorBaby _playerDetector;
    [SerializeField] private GameObject _detectedText;
    [SerializeField] private GameObject _coolDownText;

    private AttendeeMovement _attendeeMovement;
    private Coroutine _playCoroutine = null;
    private Coroutine _coolDownCoroutine = null;


    private void Awake()
    {
        _attendeeMovement = GetComponent<AttendeeMovement>();

        if (_attendeeMovement == null)
        {
            Debug.LogError("[BabyAttendee] AttendeeMovement component not found on BabyAttendee.");
        }
    }

    private void OnEnable()
    {
        _playerDetector.OnPlayerDetected += HandlePlayerDetected;
    }

    private void OnDisable()
    {
        _playerDetector.OnPlayerDetected -= HandlePlayerDetected;
    }

    private void HandlePlayerDetected(GameObject player)
    {
        if (_coolDownCoroutine != null)
        {
            return;
        }

        if (_playCoroutine != null) return;
        else
        {
            _playCoroutine = StartCoroutine(PlayWithPlayer());
        }
    }

    private IEnumerator PlayWithPlayer()
    {
        _detectedText.SetActive(true);
        if (_attendeeMovement != null)
        {
            _attendeeMovement.StopMovement();
        }

        yield return new WaitForSeconds(_playTime);

        if (_coolDownCoroutine != null)
        {
            StopCoroutine(_coolDownCoroutine);
        }
        _coolDownCoroutine = StartCoroutine(CoolDown());
        _detectedText.SetActive(false);
        PlayerEvents.RaiseOnUnfreezePlayer();
        if (_attendeeMovement != null)
        {
            _attendeeMovement.ResumeMovement();
        }
        _playCoroutine = null;
    }

    private IEnumerator CoolDown()
    {
        _coolDownText.SetActive(true);
        yield return new WaitForSeconds(_coolDownTime);
        _coolDownText.SetActive(false);
        _coolDownCoroutine = null;
    }
}
