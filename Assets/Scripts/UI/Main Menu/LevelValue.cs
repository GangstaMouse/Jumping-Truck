using UnityEngine;
using UnityEngine.UI;
using TMPro;

class LevelValue : MonoBehaviour
{
    [SerializeField] private TMP_Text m_LevelText;
    [SerializeField] private TMP_Text m_ExperienceText;
    [SerializeField] private Slider m_ExperienceProgressBar;

    private void OnEnable()
    {
        PlayerSavesManager.OnExperienceChanged += SetExperienceValue;
        SetLevelText(PlayerSavesManager.Level);
        SetExperienceValue(PlayerSavesManager.Experience);
    }

    private void OnDisable() => PlayerSavesManager.OnExperienceChanged += SetExperienceValue;

    private void SetLevelText(int value) => m_LevelText.SetText($"Level: {value}");

    private void SetExperienceValue(int value)
    {
        // Временно
        m_ExperienceText.SetText($"{value}/500");
        m_ExperienceProgressBar.maxValue = 500f;
        m_ExperienceProgressBar.value = value;
    }
}
