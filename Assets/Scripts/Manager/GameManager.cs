using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { 
        get 
        {
            return instance; 
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public void HandlePlayerFound()
    {
        Debug.Log("Player Found! Game Over.");
        // Additional game over logic here
    }
}
