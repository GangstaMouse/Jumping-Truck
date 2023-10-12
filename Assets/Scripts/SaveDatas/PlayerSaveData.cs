using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    // Переименовать в ReceivedCars/Vehicles
    public List<CarSaveData> PurchasedCars = new List<CarSaveData>();
    // При текущем функционале можно заменить на список string
    public List<CarPartSaveData> PartsReceived = new List<CarPartSaveData>();
    public string SelectedCar;
    public int Cash = 4259;
    public int Level = 1;
    public int Experience = 0;
    // Добавить реализацию в будущем
    // public int Ads = 0; Watched ads

    public string BuildVersion;
    public bool IsDevelopmentVersion = false;

    public List<RaceSaveData> RacesData = new List<RaceSaveData>();

    public RaceSaveData GetSceneData(string sceneID)
    {
        foreach (var data in RacesData)
            if (data.ID == sceneID)
                return data;

        return null;
    }

    public bool DoesSceneDataExists(string sceneID) => GetSceneData(sceneID) != null;

    // Можно попробовать упростить через GetCarSaveData != null
    public bool DoesCarReceived(string carID)
    {
        foreach (var data in PurchasedCars)
            if (data.ID == carID)
                return true;

        return false;
    }

    // Заменить CarPartData на string, partID
    public bool DoesPartReceived(CarPartData partData)
    {
        foreach (var part in PartsReceived)
            if (part.ID == partData.ID)
                return true;

        return false;
    }

    public CarSaveData GetCarSaveData(string carID)
    {
        foreach (var data in PurchasedCars)
            if (data.ID == carID)
                return data;

        return null;
    }
}
