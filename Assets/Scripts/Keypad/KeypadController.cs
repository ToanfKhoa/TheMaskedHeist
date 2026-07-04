using UnityEngine;
using TMPro; 
using UnityEngine.Events;
using System.Collections;

public class KeypadController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string correctPassword = "1234";
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private int maxLength = 4;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Events")]
    public UnityEvent OnAccessGranted; 
    public UnityEvent OnAccessDenied;  
    public UnityEvent OnGameOver;

    [SerializeField] private InteractiveObstacle linkedObstacle;

    private string _currentInput = "";
    private int _attemptsCount = 0;
    private bool _isLocked = false;

    void Start()
    {
        ResetKeypad();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRespawn.AddListener(ResetKeypad);
        }
    }

    public void PressKey(string key)
    {
        if (_isLocked || _currentInput.Length >= maxLength) return;

        _currentInput += key;
        UpdateDisplay();
    }

    public void DeleteLast()
    {
        if (_isLocked || _currentInput.Length <= 0) return;

        _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
        UpdateDisplay();
    }

    public void CheckPassword()
    {
        if (_isLocked) return;

        if (_currentInput == correctPassword)
        {
            HandleSuccess();
        }
        else
        {
            HandleFailure();
        }
    }

    private void HandleSuccess()
    {
        _isLocked = true;
        statusText.text = "ACCESS GRANTED";
        statusText.color = Color.green;

        linkedObstacle.Open();

        OnAccessGranted?.Invoke();
    }

    private void HandleFailure()
    {
        _attemptsCount++;
        _currentInput = "";
        UpdateDisplay();

        if (_attemptsCount >= maxAttempts)
        {
            _isLocked = true;
            statusText.text = "SYSTEM LOCKED";
            statusText.color = Color.red;
            OnGameOver?.Invoke();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.HandlePlayerFound();
            }
        }
        else
        {
            statusText.text = $"WRONG! ({maxAttempts - _attemptsCount} LEFT)";
            statusText.color = Color.yellow;
            OnAccessDenied?.Invoke();
            StartCoroutine(ClearStatusAfterDelay(1.5f));
        }
    }

    private void UpdateDisplay()
    {
        displayText.text = _currentInput;
    }

    private void ResetKeypad()
    {
        _currentInput = "";
        _attemptsCount = 0;
        _isLocked = false;
        displayText.text = "";
        statusText.text = "ENTER CODE";
        statusText.color = Color.white;
    }

    IEnumerator ClearStatusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!_isLocked) statusText.text = "ENTER CODE";
    }
}