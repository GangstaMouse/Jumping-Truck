using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    // set everything to private, maybe serialized?
    public List<VehicleSaveData> OwnedCars = new();
    // При текущем функционале можно заменить на список string
    public List<CarPartSaveData> OwnedParts = new();
    public List<RaceSaveData> RacesData = new();
    public string SelectedCar;
    public int Cash = 5000;
    // Добавить реализацию в будущем
    public int Level = 1;
    public int Experience = 0;
    // public int Ads = 0; Watched ads


    // v.0.1 = Release, v.0.1d = DevelopmentBuild
    public string GameVersion;
    public bool IsDevelopmentVersion => GameVersion[^1] == 'd';

    public RaceSaveData GetSavedRaceData(string raceIdentifier)
    {
        foreach (var data in RacesData)
            if (data.Identifier == raceIdentifier)
                return data;

        return null;
    }

    public bool DoesSceneDataExists(string sceneID) => GetSavedRaceData(sceneID) != null;

    public bool DoesVehicleAlreadyOwned(string vehicleIdentifier)
    {
        foreach (var data in OwnedCars)
            if (data.Identifier == vehicleIdentifier)
                return true;

        return false;
    }

    public VehicleSaveData GetSavedVehicleData(string vehicleIdentifier)
    {
        foreach (var data in OwnedCars)
            if (data.Identifier == vehicleIdentifier)
                return data;

        return null;
    }

    // Заменить CarPartData на string, partID
    public bool DoesPartAlreadyOwned(string partIdentifier)
    {
        foreach (var part in OwnedParts)
            if (part.Identifier == partIdentifier)
                return true;

        return false;
    }
}
