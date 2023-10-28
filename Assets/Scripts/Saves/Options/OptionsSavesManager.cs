using System;
using UnityEngine;

public static class OptionsSavesManager
{
    public static OptionsSaveData SaveData { get; private set; } = Load();
    private const string fileName = "Options";
    public static event Action OnAnyChanges;

    private static OptionsSaveData Load()
    {
        if (SaveSystem.DoesSaveFileExists(fileName))
            return SaveSystem.LoadFile<OptionsSaveData>(fileName);
        else
            return new OptionsSaveData();
    }

    public static void Save() => SaveSystem.SaveFile(fileName, SaveData);

    public static void ApplyOptions()
    {
        Application.targetFrameRate = 30 + (30 * SaveData.FrameRateLimit);
        QualitySettings.SetQualityLevel(SaveData.Quality);
        OnAnyChanges?.Invoke();
        Save();
    }
}
