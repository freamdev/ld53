using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene((int)Scenes.Ocean);
    }

    public void Settings()
    {
        SceneManager.LoadScene((int)Scenes.Settings);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
