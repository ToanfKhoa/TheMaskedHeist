using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneName;
    public void LoadSceneName()
    {
        SceneManager.LoadScene(sceneName);
    }
}
