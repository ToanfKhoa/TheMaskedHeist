using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] ColorChecker colorChecker;
    [SerializeField] float delayTime = 0.5f;
    [SerializeField] UnityEvent onFoundPlayer;

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
        if (checkPlayerColorCoroutine == null)
        {
            checkPlayerColorCoroutine = StartCoroutine(CheckPlayerColorAfterDelay());
        }
    }
    IEnumerator CheckPlayerColorAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        //if (colorChecker.CompareColorHSV())
    }
}
