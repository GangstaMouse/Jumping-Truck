using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CarSaveData
{
    public string ID;

    // public List<AccessorySaveData> MountedAccessories = new List<AccessorySaveData>();
    public List<InstalledPartSaveData> InstalledParts = new List<InstalledPartSaveData>();

    public string PaintHexColor = "#353535";

    public CarSaveData(string id) => ID = id;

    public void SetPartData(string socketID, CarPartData partData)
    {
        foreach (var item in InstalledParts)
            if (item.SocketID == socketID)
            {
                item.PartID = partData.ID;
                return;
            }

        InstalledParts.Add(new InstalledPartSaveData(socketID, partData.ID));
    }

    public string GetInstalledPartID(string sockedID)
    {
        foreach (var part in InstalledParts)
            if (part.SocketID == sockedID)
                return part.PartID;

        return null;
    }
}
