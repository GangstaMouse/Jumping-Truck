using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InstalledPartSaveData
{
    public string SocketID;
    public string PartID;

    public InstalledPartSaveData(string sockedID, string partID)
    {
        SocketID = sockedID;
        PartID = partID;
    }
}
