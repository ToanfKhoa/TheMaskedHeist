using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onPlayerFound;
    [SerializeField] private UnityEvent onDiamondStolen;
    [SerializeField] private UnityEvent onWin;
    [SerializeField] private UnityEvent onRespawn = new UnityEvent();

    public UnityEvent OnGameStart { get => onGameStart; }
    public UnityEvent OnPlayerFound { get => onPlayerFound; }
    public UnityEvent OnDiamondStolen { get => onDiamondStolen; }
    public UnityEvent OnWin { get => onWin; }
    public UnityEvent OnRespawn { get => onRespawn; }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private Vector2 checkpointPosition;
    private bool hasCheckpoint = false;
    private Vector2 initialPlayerPosition;
    private Camera gameplayCamera;
    private bool isLoseCutSceneLoading = false;
    public bool IsGameOver => isLoseCutSceneLoading;
    private bool diamondStolen = false;
    public bool DiamondStolen => diamondStolen;

    // Cheat test/demo: bật = player không bị phát hiện/bắt. Bật/tắt bằng phím P.
    private bool undetectable = false;
    public bool Undetectable => undetectable;

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
        gameplayCamera = Camera.main;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            initialPlayerPosition = player.transform.position;

        OnGameStart.Invoke();
        SoundManager.Instance.PlayMusic(SoundManager.Instance.partyMusic1);
    }

    private void Update()
    {
        // Cheat test/demo: P để bật/tắt khả năng bị phát hiện của player.
        if (Input.GetKeyDown(KeyCode.P))
        {
            undetectable = !undetectable;
            Debug.Log("[CHEAT] Player undetectable = " + undetectable);
        }
    }

    public void SetCheckpoint(Vector2 position)
    {
        checkpointPosition = position;
        hasCheckpoint = true;
    }

    public void HandlePlayerFound()
    {
        if (undetectable) return; // cheat test/demo: player không bị bắt
        if (isLoseCutSceneLoading) return;

        Debug.Log("Player Found! Game Over.");
        isLoseCutSceneLoading = true;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerController>().enabled = false;
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        OnPlayerFound.Invoke();
        StartCoroutine(LoadLoseCutSceneAdditive());
    }

    private IEnumerator LoadLoseCutSceneAdditive()
    {
        yield return SceneManager.LoadSceneAsync("LoseCutScene", LoadSceneMode.Additive);

        Scene cutscene = SceneManager.GetSceneByName("LoseCutScene");
        foreach (GameObject go in cutscene.GetRootGameObjects())
        {
            Camera cam = go.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.depth = gameplayCamera != null ? gameplayCamera.depth + 1 : 1;
                break;
            }
        }

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(false);
    }

    public void RespawnPlayer()
    {
        Vector2 spawnPosition = hasCheckpoint ? checkpointPosition : initialPlayerPosition;

        StartCoroutine(UnloadLoseCutScene(spawnPosition));
    }

    private IEnumerator UnloadLoseCutScene(Vector2 spawnPosition)
    {
        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(true);

        yield return SceneManager.UnloadSceneAsync("LoseCutScene");

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = spawnPosition;
            player.GetComponent<PlayerController>().enabled = true;
        }

        // Khôi phục về trạng thái trước khi nhặt kim cương
        diamondStolen = false;
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMusic(SoundManager.Instance.partyMusic1);
        OnRespawn.Invoke();

        isLoseCutSceneLoading = false;
    }

    public void HandleDiamondStolen()
    {
        diamondStolen = true;
        Debug.Log("Diamond Stolen! Alert!");
        OnDiamondStolen.Invoke();
        // Additional alert logic here
    }

    public void HandleWin()
    {
        Debug.Log("You Win!");
        OnWin.Invoke();
        // Additional win logic here
        SoundManager.Instance.PlayMusic(SoundManager.Instance.winMusic);
        SceneManager.LoadScene("WinScene");
    }
}
