using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour
{
    private VolumeProfile m_VolumeProfile;
    private Dictionary<string, bool> m_Effects;

    private void Awake()
    {
        Volume volume = GetComponent<Volume>();
        m_VolumeProfile = volume.profile;

        SetEffects();
    }

    private void SetEffects()
    {
        m_Effects = new Dictionary<string, bool>
        {
            { "Tonemapping", OptionsSavesManager.SaveData.ColorCorrection },
            { "ColorAdjustments", OptionsSavesManager.SaveData.ColorCorrection },
            { "Vignette", OptionsSavesManager.SaveData.Vignette },
            { "Bloom", OptionsSavesManager.SaveData.Bloom }
        };

        foreach (var item in m_VolumeProfile.components)
            item.active = GetVal(item.name);

        m_Effects.Clear();
    }

    private bool GetVal(string name)
    {
        foreach (var effectName in m_Effects.Keys)
            if (Regex.IsMatch(name, effectName))
                return m_Effects[effectName];

        return false;
    }

    private void OnEnable() => OptionsSavesManager.OnAnyChanges += SetEffects;

    private void OnDisable() => OptionsSavesManager.OnAnyChanges -= SetEffects;
}
