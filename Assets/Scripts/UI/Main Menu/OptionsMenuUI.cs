using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OptionsMenuUI : BaseUI
{
    [Header("References")]
    [SerializeField] private TMP_Dropdown framerateDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    // Post Processing
    [SerializeField] private Toggle colorCorrectionToggle;
    [SerializeField] private Toggle vignetteToggle;
    [SerializeField] private Toggle bloomToggle;

    private readonly List<string> qualityLevels = new() { "Low", "Medium", "High", "Ultra"};

    private bool menuVisibility;

    protected override void Awake()
    {
        base.Awake();
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
            DrawUI();
    }

    protected override void DrawUI()
    {
        framerateDropdown.SetValueWithoutNotify(OptionsSavesManager.SaveData.FrameRateLimit);
        qualityDropdown.SetValueWithoutNotify(OptionsSavesManager.SaveData.Quality);

        // Post Processing
        colorCorrectionToggle.SetIsOnWithoutNotify(OptionsSavesManager.SaveData.ColorCorrection);
        vignetteToggle.SetIsOnWithoutNotify(OptionsSavesManager.SaveData.Vignette);
        bloomToggle.SetIsOnWithoutNotify(OptionsSavesManager.SaveData.Bloom);
    }

    private void OptionsChanged()
    {
        Debug.Log("Options changed");

        OptionsSavesManager.SaveData.FrameRateLimit = framerateDropdown.value;
        OptionsSavesManager.SaveData.Quality = qualityDropdown.value;

        // Post Processing
        OptionsSavesManager.SaveData.ColorCorrection = colorCorrectionToggle.isOn;
        OptionsSavesManager.SaveData.Vignette = vignetteToggle.isOn;
        OptionsSavesManager.SaveData.Bloom = bloomToggle.isOn;

        OptionsSavesManager.Save();
        OptionsSavesManager.ApplyOptions();
    }
}
