using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerSavesManager
{
    private static PlayerSaveData SaveData = Load();
    private const string fileName = "Player";

    public static int Cash => SaveData.Cash;
    public static int Level => SaveData.Level;
    public static int Experience => SaveData.Experience;

    public static event Action<int> OnCashChanged;
    public static event Action<int> OnLevelChanged;
    public static event Action<int> OnExperienceChanged;

    private static PlayerSaveData Load()
    {
        if (SaveSystem.DoesSaveFileExists(fileName))
        {
            PlayerSaveData loadedSaveData = SaveSystem.LoadFile<PlayerSaveData>(fileName);
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
            SaveSystem.DeleteFile(fileName);
            saveData = new PlayerSaveData();
        }

#if DEVELOPMENT_BUILD
        saveData.Gameversion = $"{Application.version}d";
#else
        saveData.GameVersion = Application.version;
#endif
        // saveData.IsDevelopmentVersion = Debug.isDebugBuild;

        if (saveData != originalSaveData)
            OverrideSave(saveData);

        return saveData;
    }

    private static void OverrideSave(PlayerSaveData data) => SaveSystem.SaveFile(fileName, data);

    public static void Save() => SaveSystem.SaveFile(fileName, SaveData);

    private static void SaveQueue() { }

    #region Basics
    private static void SpendMoney(int value)
    {
        if (value <= 0 || Cash < value)
            return;

        SaveData.Cash -= value;
        OnCashChanged?.Invoke(Cash);
    }

    private static void ReceiveMoney(int value)
    {
        if (value <= 0)
            return;

        SaveData.Cash += value;
        OnCashChanged?.Invoke(Cash);
    }

    private static void ReceiveExperience(int value)
    {
        if (value <= 0)
            return;

        SaveData.Experience += value;
        OnExperienceChanged?.Invoke(SaveData.Experience);
        // need level implementation
    }
    #endregion

    #region Vehicle

    public static partial class Vehicle
    {
        public static string SelectedCarIdentifier => SaveData.SelectedCar;

        public static bool DoesVehicleOwned(CarDataSO dataAsset) => DoesVehicleOwned(dataAsset.Identifier);
        public static bool DoesVehicleOwned(string Identifier)
        {
            if (string.IsNullOrEmpty(Identifier))
                return false;

            foreach (var vehicleData in SaveData.OwnedCars)
                if (Identifier == vehicleData.Identifier)
                    return true;

            return false;
        }

        public static void SelectVehicle(CarDataSO dataAsset)
        {
            if (dataAsset != null && string.IsNullOrEmpty(dataAsset.Identifier) == false && DoesVehicleOwned(dataAsset))
            {
                SaveData.SelectedCar = dataAsset.Identifier;
                Save();
                return;
            }

            Debug.LogError("Trying select invalid vehicle");
        }

        public static bool PurchaseVehicle(CarDataSO dataAsset)
        {
            if (dataAsset != null && DoesVehicleOwned(dataAsset) == false && dataAsset.Price <= SaveData.Cash)
            {
                SpendMoney(dataAsset.Price);
                string hexColor = ColorUtility.ToHtmlStringRGB(dataAsset.DefaultPaintAsset.Color);

                VehicleSaveData carSaveData = new(dataAsset.Identifier)
                {
                    PaintHexColor = hexColor
                };
                SaveData.OwnedCars.Add(carSaveData);

                if (string.IsNullOrEmpty(SaveData.SelectedCar))
                    SaveData.SelectedCar = dataAsset.Identifier;

                Save();
            }

            Debug.LogWarning("Cant purchase vehicle");

            return false;
        }

        public static partial class Upgrades
        {
            public static class Utils
            {
                public static void LoadPlayerCarTuning(GameObject carObject, string carID)
                {
                    // Load Upgrades
                    List<PartSocket> partSockets = new(carObject.GetComponentsInChildren<PartSocket>());

                    foreach (var socket in partSockets)
                    {
                        string partID = GetInstalledPartID(carID, socket.ID);

                        if (string.IsNullOrEmpty(partID) == false)
                        {
                            BasePart partData = ItemDataSO.LoadAssetFromIdentifier<BasePart>(partID, "Upgrades");

                            if (partData != null)
                                socket.Install(partData);
                        }
                    }

                    LoadPlayerCarColor(carObject);
                }

                public static void LoadPlayerCarColor(GameObject carObject)
                {
                    // Load Paint Color
                    string hexColor = GetCarPaintHexColor(SelectedCarIdentifier);

                    if (ColorUtility.TryParseHtmlString(hexColor, out Color paintColor) == false)
                        Debug.LogWarning($"Hex Color <<{hexColor}>> is invalid");

                    PaintCar(carObject, paintColor);
                }

                public static void PaintCar(GameObject carObject, Color color)
                {
                    List<Material> materials = FunctionsLibrary.GetListOfMaterialsByName(carObject, "Car Paint");

                    foreach (var material in materials)
                        material.color = color;
                }
            }
            public static void SelectVehiclePart(string vehicleIdentifier, string sockedIdentifier, BasePart dataAsset)
            {
                if (DoesVehicleOwned(vehicleIdentifier) && string.IsNullOrEmpty(sockedIdentifier) == false && SaveData.DoesPartAlreadyOwned(dataAsset.Identifier))
                {
                    VehicleSaveData carSaveData = SaveData.GetSavedVehicleData(vehicleIdentifier);
                    carSaveData.SetPartData(sockedIdentifier, dataAsset);
                    Save();
                }
            }

            public static bool PurchaseVehiclePart(BasePart assetData)
            {
                if (assetData != null && SaveData.DoesPartAlreadyOwned(assetData.Identifier) == false && assetData.Price <= SaveData.Cash)
                {
                    SpendMoney(assetData.Price);
                    AddVehiclePart(assetData);
                    Save();
                    return true;
                }

                return false;
            }

            public static string GetInstalledPartID(string vehicleIdentifier, string socketIdentifier) => SaveData.GetSavedVehicleData(vehicleIdentifier).GetInstalledPartID(socketIdentifier);

            public static bool DoesPartReceived(BasePart dataAsset) => SaveData.DoesPartAlreadyOwned(dataAsset.Identifier);

            public static void AddVehiclePart(BasePart dataAsset) => SaveData.OwnedParts.Add(new CarPartSaveData(dataAsset.Identifier));
        }
        #endregion

        #region Paint

        public static string GetCarPaintHexColor(string vehicleIdentifier)
        {
            if (SaveData.DoesVehicleAlreadyOwned(vehicleIdentifier))
                return $"#{SaveData.GetSavedVehicleData(vehicleIdentifier).PaintHexColor}";

            return null;
        }

        public static bool PurchasePaint(string vehicleIdentifier, PaintDataSO dataAsset)
        {
            if (DoesVehicleOwned(vehicleIdentifier) == false)
                return false;

            VehicleSaveData vehicleSave = SaveData.GetSavedVehicleData(vehicleIdentifier);
            string hexColor = ColorUtility.ToHtmlStringRGB(dataAsset.Color);

            if (hexColor != vehicleSave.PaintHexColor && dataAsset.Price <= SaveData.Cash)
            {
                SpendMoney(Cash);
                vehicleSave.PaintHexColor = hexColor;

                Save();
                return true;
            }

            return false;
        }
    }
    #endregion

    #region Race

    public static partial class Race
    {
        public static void WriteRaceCompletion(RaceData raceData, TimeSpan raceTime, int raceScore)
        {
            long prevRaceTime;
            int prevRaceScore;
            ReadRaceSaveData(raceData.Identifier, out prevRaceTime, out prevRaceScore);

            List<string> requirements = new(raceData.Goal.Replace(" ", null).Split(','));

            // Opening buffer
            RaceSaveDataBuffer buffer = new(prevRaceTime, prevRaceScore);

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
            WriteRaceSaveData(raceData.Identifier, buffer);

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

            RaceSaveData raceSave = SaveData.GetSavedRaceData(id);

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

            RaceSaveData raceSave = SaveData.GetSavedRaceData(id);

            if (raceSave == null)
            {
                raceSave = new RaceSaveData(id);
                SaveData.RacesData.Add(raceSave);
            }

            raceSave.BestTimeTicks = bufferData.Time;
            raceSave.BestScore = bufferData.Score;
        }
    }

    #endregion
}
