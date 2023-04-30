using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    private void Awake()
    {
        GameObject.Find("VolumeSlider").GetComponent<Slider>().value = OptionsProvider.Get().AudioStrength;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene((int)Scenes.MainMenu);
    }

    //did not read
    public void ValueChange(float v)
    {
        OptionsProvider.Get().AudioStrength = v;
    }
}
