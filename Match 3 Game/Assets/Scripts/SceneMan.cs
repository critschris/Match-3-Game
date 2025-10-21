using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMan : MonoBehaviour
{
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void GoToLevel1()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }
}
