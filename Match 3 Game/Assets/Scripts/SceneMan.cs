using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMan : MonoBehaviour
{
    public void PauseGame()
    {
        FindFirstObjectByType<InputManager>().paused_game = true;
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        FindFirstObjectByType<InputManager>().paused_game = false;
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

    public void GoToLevel2()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    
}
