using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button m_PauseButton;
    private bool m_IsVisible;

    public void SetVisibity(bool visibility)
    {
        m_IsVisible = visibility;

        GetComponent<OptionsMenuUI>().SetVisibility(m_IsVisible);
        m_PauseButton.gameObject.SetActive(!m_IsVisible);

        ChangeTimeSpeed(m_IsVisible ? 0f : 1f);
    }

    private void ChangeTimeSpeed(float value) => Time.timeScale = value;

    public void ToggleVisibility() => SetVisibity(!m_IsVisible);

    // public void ReturnButton() => SetVisibity(false);

    public void RestartButton()
    {
        ChangeTimeSpeed(1f);
        SceneLoader.ReloadScene();
    }

    public void MainMenuButton()
    {
        ChangeTimeSpeed(1f);
        SceneLoader.LoadScene("MainMenu");
    }
}
