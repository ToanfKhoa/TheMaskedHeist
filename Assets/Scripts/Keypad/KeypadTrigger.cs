using UnityEngine;

public class KeypadTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject keypadCanvas; 

    private bool _isPlayerInRange = false;

    void Start()
    {
        if (keypadCanvas) keypadCanvas.SetActive(false);
    }

    void Update()
    {
        if (_isPlayerInRange && !_isKeypadOpen())
        {
            OpenKeypad();
        }
    }

    private void OpenKeypad()
    {
        keypadCanvas.SetActive(true);
    }

    public void CloseKeypad()
    {
        keypadCanvas.SetActive(false);
        _isPlayerInRange = false; 
    }

    private bool _isKeypadOpen() => keypadCanvas.activeSelf;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;
            if (keypadCanvas) CloseKeypad();
        }
    }
    public void CloseKeyPad()
    {
        keypadCanvas.SetActive(true);
        _isPlayerInRange = false;
    }
}