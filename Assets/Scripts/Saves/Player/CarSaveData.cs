using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class VehicleSaveData
{
    [SerializeField] private string m_Identifier;
    [SerializeField] private string m_PaintHexColor = "#353535";
    public string Identifier => m_Identifier;
    public string PaintHexColor { get => m_PaintHexColor; internal set => m_PaintHexColor = value; }
    [SerializeField] internal List<InstalledPartSaveData> InstalledParts = new();

    public VehicleSaveData(string identifier) => m_Identifier = identifier;

    public void SetPartData(string socketID, BasePart partData)
    {
        foreach (var item in InstalledParts)
            if (item.SocketID == socketID)
            {
                item.PartID = partData.Identifier;
                return;
            }

        InstalledParts.Add(new InstalledPartSaveData(socketID, partData.Identifier));
    }

    public string GetInstalledPartID(string sockedID)
    {
        foreach (var part in InstalledParts)
            if (part.SocketID == sockedID)
                return part.PartID;

        return null;
    }
}
