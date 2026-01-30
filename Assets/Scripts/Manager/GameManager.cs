using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onPlayerFound;
    [SerializeField] private UnityEvent onDiamondStolen;

    public UnityEvent OnPlayerFound { get => onPlayerFound; }
    public UnityEvent OndiamondStolen { get => onDiamondStolen; }

    private static GameManager instance;
    public static GameManager Instance
    {
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

    public void HandleDiamondStolen()
    {
        Debug.Log("Diamond Stolen! Alert!");
        // Additional alert logic here
    }
}
