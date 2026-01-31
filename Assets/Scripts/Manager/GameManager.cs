using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onPlayerFound;
    [SerializeField] private UnityEvent onDiamondStolen;
    [SerializeField] private UnityEvent onWin;

    public UnityEvent OnGameStart { get => onGameStart; }
    public UnityEvent OnPlayerFound { get => onPlayerFound; }
    public UnityEvent OnDiamondStolen { get => onDiamondStolen; }
    public UnityEvent OnWin { get => onWin; }

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

    private void Start()
    {
        OnGameStart.Invoke();
        SoundManager.Instance.PlayMusic(SoundManager.Instance.partyMusic1);
    }

    public void HandlePlayerFound()
    {
        Debug.Log("Player Found! Game Over.");
        OnPlayerFound.Invoke();
        // Additional game over logic here
    }

    public void HandleDiamondStolen()
    {
        Debug.Log("Diamond Stolen! Alert!");
        OnDiamondStolen.Invoke();
        // Additional alert logic here
    }

    public void HandleWin()
    {
        Debug.Log("You Win!");
        OnWin.Invoke();
        // Additional win logic here
    }
}
