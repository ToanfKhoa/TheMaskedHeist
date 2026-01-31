using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic; // Add this using directive

public class RoomManager : MonoBehaviour
{
    public RoomBasedCamera cam;
    public List<GameObject> enemiesGroup;

    //void OnEnable()
    //{
    //    // Subscribe to the events
    //    cam.OnTransitionStarted += DisableEnemies;
    //    cam.OnTransitionReached += EnableEnemies;
    //}

    //void OnDisable()
    //{
    //    // Always unsubscribe to prevent memory leaks
    //    cam.OnTransitionStarted -= DisableEnemies;
    //    cam.OnTransitionReached -= EnableEnemies;
    //}

    private void Start()
    {
        DisableEnemies();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EnableEnemies();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DisableEnemies();
        }
    }

    void DisableEnemies() 
    { 
        Debug.Log("Camera moving! Unloading"); 
        foreach (var group in enemiesGroup)
        {
            group.SetActive(false);
        }
    }
    void EnableEnemies() 
    { 
        Debug.Log("Camera arrived! Spawning enemies..."); 
        foreach (var group in enemiesGroup)
        {
            group.SetActive(true);
        }
    }
}