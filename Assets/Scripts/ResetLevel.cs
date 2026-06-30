using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour
{
    public string gamePlaySceneName;

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RespawnPlayer();
        else
            SceneManager.LoadScene(gamePlaySceneName);
    }
}
