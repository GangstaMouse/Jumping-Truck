using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccessorySaveData
{
    public string ID;
    public string MountKey;

    public AccessoryData GetAccessoryData()
    {
        List<AccessoryData> accessorysData = new List<AccessoryData>(Resources.LoadAll<AccessoryData>("Accessories"));

        foreach (var data in accessorysData)
            if (data.ID == ID)
                return data;

        return null;
    }
}
