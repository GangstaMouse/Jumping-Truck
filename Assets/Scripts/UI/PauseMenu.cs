using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButtonObject;

    public bool Visibility { get; private set; }

    public void SetVisibity(bool newVisibility)
    {
        Visibility = newVisibility;

        // gameObject.SetActive(Visibility);
        GetComponent<OptionsMenuUI>().SetVisibility(Visibility);
        pauseButtonObject.SetActive(!Visibility);

        ChangeTimeSpeed(Visibility ? 0f : 1f);
    }

    private void ChangeTimeSpeed(float value) => Time.timeScale = value;

    public void ToggleVisibility() => SetVisibity(!Visibility);

    // public void ReturnButton() => SetVisibity(false);

    public void RestartButton()
    {
        ChangeTimeSpeed(1f);
        SceneLoader.instance.ReloadScene();
    }

    public void MainMenuButton()
    {
        ChangeTimeSpeed(1f);
        SceneLoader.instance.LoadScene("MainMenu");
    }
}
