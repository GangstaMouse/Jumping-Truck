using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour
{
    private VolumeProfile volumeProfile;
    private Dictionary<string, bool> effects;

    private void Awake()
    {
        Volume volume = GetComponent<Volume>();
        volumeProfile = volume.profile;

        SetEffects();
    }

    private void SetEffects()
    {
        effects = new Dictionary<string, bool>();

        effects.Add("Tonemapping", OptionsDataProcessor.ColorCorrectionValue);
        effects.Add("ColorAdjustments", OptionsDataProcessor.ColorCorrectionValue);
        effects.Add("Vignette", OptionsDataProcessor.VignetteValue);
        effects.Add("Bloom", OptionsDataProcessor.BloomValue);

        // Debug.LogWarning(Regex.IsMatch("Bloon", "Bloom"));

        foreach (var item in volumeProfile.components)
        {
            /* if (Regex.IsMatch(item.name, "Bloom"))
                item.active = false; */

            Debug.LogWarning(item.name);

            item.active = GetVal(item.name);
        }

        effects.Clear();
    }

    private bool GetVal(string name)
    {
        foreach (var effectName in effects.Keys)
            if (Regex.IsMatch(name, effectName))
                return effects[effectName];

        return false;
    }

    private void OnEnable() => OptionsDataProcessor.OnOptionChanged += SetEffects;

    private void OnDisable() => OptionsDataProcessor.OnOptionChanged -= SetEffects;
}
