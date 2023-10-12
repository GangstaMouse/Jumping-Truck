using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OptionsMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Dropdown framerateDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    // Post Processing
    [SerializeField] private Toggle colorCorrectionToggle;
    [SerializeField] private Toggle vignetteToggle;
    [SerializeField] private Toggle bloomToggle;

    private readonly List<string> qualityLevels = new List<string>{"Low", "Medium", "High", "Ultra"};

    private bool menuVisibility;

    private void Awake()
    {
        framerateDropdown.onValueChanged.AddListener(delegate { OptionsChanged(); });
        qualityDropdown.onValueChanged.AddListener(delegate { OptionsChanged(); });

        // Post Processing
        colorCorrectionToggle.onValueChanged.AddListener(delegate { OptionsChanged(); });
        vignetteToggle.onValueChanged.AddListener(delegate { OptionsChanged(); });
        bloomToggle.onValueChanged.AddListener(delegate { OptionsChanged(); });

        qualityDropdown.options.Clear();

        foreach (var qualityLevel in qualityLevels)
            qualityDropdown.options.Add(new TMP_Dropdown.OptionData(qualityLevel));
    }

    public void ToggleVisibility() => SetVisibility(!menuVisibility);

    public void SetVisibility(bool newVisibility)
    {
        menuVisibility = newVisibility;
        gameObject.SetActive(menuVisibility);

        if (menuVisibility)
            RefreshOptions();
    }

    private void RefreshOptions()
    {
        framerateDropdown.SetValueWithoutNotify(OptionsDataProcessor.FramerateLimitValue);
        qualityDropdown.SetValueWithoutNotify(OptionsDataProcessor.QualityValue);

        // Post Processing
        colorCorrectionToggle.SetIsOnWithoutNotify(OptionsDataProcessor.ColorCorrectionValue);
        vignetteToggle.SetIsOnWithoutNotify(OptionsDataProcessor.VignetteValue);
        bloomToggle.SetIsOnWithoutNotify(OptionsDataProcessor.BloomValue);
    }

    private void OptionsChanged()
    {
        Debug.Log("Options changed");

        OptionsDataProcessor.FramerateLimitValue = framerateDropdown.value;
        OptionsDataProcessor.QualityValue = qualityDropdown.value;

        // Post Processing
        OptionsDataProcessor.ColorCorrectionValue = colorCorrectionToggle.isOn;
        OptionsDataProcessor.VignetteValue = vignetteToggle.isOn;
        OptionsDataProcessor.BloomValue = bloomToggle.isOn;

        OptionsDataProcessor.Save();
        OptionsDataProcessor.ApplyOptions();
    }
}
