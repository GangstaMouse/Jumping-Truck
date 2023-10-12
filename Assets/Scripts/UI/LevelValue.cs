using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelValue : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private Slider experienceProgressBar;

    private void OnEnable()
    {
        // PlayerDataProcessor.OnExperienceValueChanged += SetExperienceValue;
        SetLevelText(PlayerDataProcessor.GetLevelValue);
        SetExperienceValue(PlayerDataProcessor.GetExperienceValue);
    }

    // private void OnDisable() => PlayerDataProcessor.OnExperienceValueChanged += SetExperienceValue;

    private void SetLevelText(int value) => levelText.SetText($"Level: {value}");

    private void SetExperienceValue(int value)
    {
        // Временно
        experienceText.SetText($"{value}/500");
        experienceProgressBar.maxValue = 500f;
        experienceProgressBar.value = value;
    }
}
