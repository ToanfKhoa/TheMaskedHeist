using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour
{
    public string gamePlaySceneName;
    void Start()
    {
        SceneManager.LoadScene(gamePlaySceneName);
    }
}
