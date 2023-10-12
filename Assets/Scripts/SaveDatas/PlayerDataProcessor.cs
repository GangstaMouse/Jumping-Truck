using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerDataProcessor
{
    private static PlayerSaveData saveData = Load();

    public static List<CarSaveData> PurchasedCars => saveData.PurchasedCars;

    private const string fileName = "Player";

    public static event Action<int> OnCashValueChanged;
    // Я думаю что это не пригодится, ведь сейчас нету случая в котором она нужна
    public static event Action<int> OnExperienceValueChanged;


    private static PlayerSaveData Load()
    {
        if (SaveGame.DoesSaveFileExists(fileName))
        {
            PlayerSaveData loadedSaveData = (PlayerSaveData)SaveGame.LoadFile(fileName, typeof(PlayerSaveData));
            return VerifySavedData(loadedSaveData);
        }

        return new PlayerSaveData();
    }

    private static PlayerSaveData VerifySavedData(PlayerSaveData loadedSaveData)
    {
        PlayerSaveData originalSaveData = loadedSaveData;
        PlayerSaveData saveData = originalSaveData;

        if (!Debug.isDebugBuild && saveData.IsDevelopmentVersion)
        {
            // Пока не уверен что сейчас это лучше делать
            // SaveGame.DeleteFile(fileName);
            saveData = new PlayerSaveData();
        }

        saveData.BuildVersion = Application.version;
        saveData.IsDevelopmentVersion = Debug.isDebugBuild;

        // Нужно проверить как удаляются элементы
        /* foreach (var data in playerSaveData.RacesData)
            if (string.IsNullOrEmpty(data.ID))
                playerSaveData.RacesData.Remove(data); */

        // Добавить чистку файла, удаление пустых значений

        if (saveData != originalSaveData)
            OverrideSave(saveData);

        return saveData;
    }

    // Нужно удостовериться в работоспособности
    // private static void OverrideSave(PlayerSaveData saveData) => SaveGame.SaveFile(fileName, saveData);
    private static void OverrideSave(PlayerSaveData data) => SaveGame.SaveFile(fileName, data);

    public static void Save() => SaveGame.SaveFile(fileName, saveData);

    private static void SaveQueue()
    {
        
    }

#region Basics

    private static void SpendMoney(int value)
    {
        if (value <= 0 || GetCash() < value)
            return;

        saveData.Cash -= value;
        OnCashValueChanged?.Invoke(GetCash());
    }

    private static void ReceiveMoney(int value)
    {
        if (value <= 0)
            return;

        saveData.Cash += value;
        OnCashValueChanged?.Invoke(GetCash());
    }

    private static void ReceiveExperience(int value)
    {
        if (value <= 0)
            return;

        saveData.Experience += value;
        OnExperienceValueChanged?.Invoke(saveData.Experience);
    }

    #endregion

#region Car

    public static bool DoesCarPurchased(CarData carData)
    {
        foreach (var car in saveData.PurchasedCars)
            if (car.ID == carData.ID)
                return true;

        return false;
    }

    public static bool DoesCarSelected() => string.IsNullOrEmpty(saveData.SelectedCar);

    public static bool DoesThisCarSelected(CarData carData) => carData.ID == saveData.SelectedCar;

    public static void SetSelectedCar(CarData carData)
    {
        if (!DoesCarPurchased(carData))
            return;

        saveData.SelectedCar = carData.ID;

        // Добавить сохранение
        Save();
    }

    // private static void SpendMoney --

    // private static void ReceiveMoney ++

    public static bool BuyCar(CarData carData)
    {
        if (DoesCarPurchased(carData) || saveData.Cash < carData.Price)
            return false;

        saveData.Cash = saveData.Cash - carData.Price;
        // string hexColor = $"#{ColorUtility.ToHtmlStringRGB(Color.white)}";
        string hexColor = $"#{ColorUtility.ToHtmlStringRGB(carData.DefaultPaintingAsset.Color)}";

        // Test
        Debug.LogWarning(hexColor);

        CarSaveData newCarSaveData = new CarSaveData(carData.ID);
        newCarSaveData.PaintHexColor = hexColor;
        saveData.PurchasedCars.Add(newCarSaveData);
        // ---

        // saveData.PurchasedCars.Add(new CarSaveData(carData.ID, hexColor));
        OnCashValueChanged?.Invoke(saveData.Cash);

        if (string.IsNullOrEmpty(saveData.SelectedCar))
            saveData.SelectedCar = carData.ID;

        // Save();

        return true;
    }

    private static void SelectDefaultCarParts(CarData carData)
    {
        
    }

    private static void GetFreePartsFromCar(CarData carData)
    {
        List<CarPartData> upgrades = new List<CarPartData>(Resources.LoadAll<CarPartData>("Upgrades"));

        foreach (var upgrade in upgrades)
        {
            if (!DoesPartReceived(upgrade) && upgrade.Price == 0 && string.IsNullOrEmpty(upgrade.UnlockRequirements))
            {
                GiveCarPart(upgrade);
            }
        }
    }

    public static int GetCash() => saveData.Cash;

    public static CarData GetSelectedCarData()
    {
        // Думаю можно это убрать
        if (string.IsNullOrEmpty(saveData.SelectedCar))
            return null;

        foreach (var car in saveData.PurchasedCars)
            if (car.ID == saveData.SelectedCar)
                return CarData.GetAssetByID(car.ID);

        return null;
    }

    public static string GetSelectedCarID => saveData.SelectedCar;

    #endregion

#region Upgrade

    public static CarPartData GetCarPartData(string carID, string socketID)
    {
        CarSaveData carSaveData = saveData.GetCarSaveData(carID);

        foreach (var saveData in carSaveData.InstalledParts)
            if (saveData.SocketID == socketID)
                return CarPartData.GetAssetByID(saveData.PartID);

        return null;
    }

    public static void SetCarPartData(string carID, string sockedID, CarPartData partData)
    {
        CarSaveData carSaveData = saveData.GetCarSaveData(carID);
        carSaveData.SetPartData(sockedID, partData);
    }

    public static int GetLevelValue => saveData.Level;

    public static int GetExperienceValue => saveData.Experience;



    public static bool BuyCarPart(CarPartData partData)
    {
        if (saveData.DoesPartReceived(partData) || saveData.Cash < partData.Price)
            return false;

        saveData.Cash -= partData.Price;
        GiveCarPart(partData);

        return true;
    }

    public static string GetInstalledPartID(string carID, string sockedID) => saveData.GetCarSaveData(carID).GetInstalledPartID(sockedID);

    public static bool DoesPartReceived(CarPartData partData) => saveData.DoesPartReceived(partData);

    // Заменить carpartsaveData
    // Переименовать
    public static void GiveCarPart(CarPartData partData) => saveData.PartsReceived.Add(new CarPartSaveData(partData.ID));

    #endregion

#region Paint

    // public static string GetCarPaintHexColor(string carID) => saveData.GetCarSaveData(carID).PaintHexColor;
    // Разобраться как где лучше разместить добавление решетки
    [System.Obsolete]
    public static string GetCarPaintHexColor(string carID) => $"#{saveData.GetCarSaveData(carID).PaintHexColor}";

    public static bool BuyCarPaint(string carID, PaintingColorData paintingColorData)
    {
        CarSaveData carSaveData = saveData.GetCarSaveData(carID);
        string hexColor = ColorUtility.ToHtmlStringRGB(paintingColorData.Color);

        if (carSaveData.PaintHexColor == hexColor || saveData.Cash < paintingColorData.Price)
            return false;

        // saveData.Cash -= paintingColorData.Price;
        carSaveData.PaintHexColor = hexColor;

        // Save();
        return true;
    }

    /* private static void SetCarPaintHexColor(string carID, string hexColor)
    {
        saveData.GetCarSaveData(carID).PaintHexColor = hexColor;
        // Save();
    } */

    #endregion

#region Race

    public static void WriteRaceCompletion(RaceData raceData, TimeSpan raceTime, int raceScore)
    {
        long prevRaceTime;
        int prevRaceScore;
        ReadRaceSaveData(raceData.ID, out prevRaceTime, out prevRaceScore);

        List<string> requirements = new List<string>(raceData.Goal.Replace(" ", null).Split(','));

        // Opening buffer
        RaceSaveDataBuffer buffer = new RaceSaveDataBuffer(prevRaceTime, prevRaceScore);

        // Реализовано пока что только условие И, позже добавить условие ИЛИ ( || )
        foreach (var requirement in requirements)
        {
            bool requirementPassed = false;
            string condition;
            string value;

            FunctionsLibrary.GetValuesFromCommand(requirement, out condition, out value);

            // Successfully
            switch (condition)
            {
                case "time":
                    if (raceTime <= TimeSpan.Parse(value))
                    {
                        requirementPassed = true;
                        buffer.Time = raceTime.Ticks;
                    }
                    break;

                case "score":
                    if (raceScore >= int.Parse(value))
                    {
                        requirementPassed = true;
                        buffer.Score = raceScore;
                    }
                    break;
            }

            // Warning! Это работает только с условием И
            if (!requirementPassed)
                return;
        }

        // Write race data from buffer
        WriteRaceSaveData(raceData.ID, buffer);

        ReceiveMoney(raceData.CashReward);
        ReceiveExperience(raceData.ExperienceReward);
        Save();
    }

    private struct RaceSaveDataBuffer
    {
        public long Time;
        public int Score;

        public RaceSaveDataBuffer(long time, int score)
        {
            Time = time;
            Score = score;
        }
    }

    public static void ReadRaceSaveData(string id, out long tickTime, out int score)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException("raceID is null or empty");

        RaceSaveData raceSave = saveData.GetSceneData(id);

        if (raceSave == null)
        {
            tickTime = 0;
            score = 0;
            return;
        }

        tickTime = raceSave.BestTimeTicks;
        score = raceSave.BestScore;
    }

    private static void WriteRaceSaveData(string id, RaceSaveDataBuffer bufferData)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException("raceID is null or empty");

        RaceSaveData raceSave = saveData.GetSceneData(id);

        if (raceSave == null)
        {
            raceSave = new RaceSaveData(id);
            saveData.RacesData.Add(raceSave);
        }

        raceSave.BestTimeTicks = bufferData.Time;
        raceSave.BestScore = bufferData.Score;
    }

    #endregion
}
