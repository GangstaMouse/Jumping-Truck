using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OptionsDataProcessor
{
    private static OptionsSaveData saveData = Load();

    private const string fileName = "Options";

    public static event Action OnOptionChanged;

    // Test 1
    private const float saveTimeDelay = 2f;
    private static OptionsSaveData bufferData;
    private static float saveTime;
    private static CancellationTokenSource tokenSource = new CancellationTokenSource();
    private static Task task;
    // ---

    private static OptionsSaveData Load()
    {
        if (SaveGame.DoesSaveFileExists(fileName))
            return (OptionsSaveData)SaveGame.LoadFile(fileName, typeof(OptionsSaveData));
        else
            return new OptionsSaveData();
    }

    // public static void Save() => SaveGame.SaveFile(fileName, saveData);

    // Test 1
    // Переименовать в RequestSave
    public static void Save()
    {
        // Variant 2
        /* if (task == null)
            bufferData = new OptionsSaveData(); */

        if (task != null)
        {
            // Variant 1
            saveTime = 0f;

            // Variant 2
            /* tokenSource.Cancel();
            task.Dispose();
            task = null; */
        }

        // Token = variant 2
        tokenSource = new CancellationTokenSource();
        task = TestBufferZone(tokenSource.Token);
    }

    private static async Task TestBufferZone(CancellationToken token)
    {
        Debug.Log("TestBufferZone - started");

        // Variant 1
        bufferData = new OptionsSaveData();

        while (saveTime < saveTimeDelay)
        {
            saveTime += Time.unscaledDeltaTime;
            await Task.Yield();
        }
        Debug.Log($"TestBufferZone - completed with time {saveTime}");
        
        // Variant 2
        /* await Task.Delay(TimeSpan.FromSeconds(saveTimeDelay), token);
        Debug.Log("TestBufferZone - completed"); */

        SaveGame.SaveFile(fileName, saveData);
        task = null;
        bufferData = null;

        // Varint 1
        saveTime = 0f;
    }
    // ---

    public static void ApplyOptions()
    {
        Debug.Log("Options apply");
        Application.targetFrameRate = 30 + (30 * saveData.FramerateLimit);
        QualitySettings.SetQualityLevel(saveData.Quality);
        OnOptionChanged?.Invoke();
    }

    // Warning!!! Убрать сохранение при изменении параметров, сохранять из меню настроек
    // Все GetOptionValue можно заменить на переменные, убрать скобки присущие методу. Сделано!
    public static int FramerateLimitValue { get => saveData.FramerateLimit; set => saveData.FramerateLimit = value; }
    public static int QualityValue { get => saveData.Quality; set => saveData.Quality = value; }

#region Post Processing

    public static bool ColorCorrectionValue { get => saveData.ColorCorrection; set => saveData.ColorCorrection = value; }
    public static bool VignetteValue { get => saveData.Vignette; set => saveData.Vignette = value; }
    public static bool BloomValue { get => saveData.Bloom; set => saveData.Bloom = value; }

#endregion

#region Controls

    public static int PedalsType { get => saveData.PedalsType; set => saveData.PedalsType = value; }
    public static int SteeringType { get => saveData.SteeringType; set => saveData.SteeringType = value; }

#endregion
}
