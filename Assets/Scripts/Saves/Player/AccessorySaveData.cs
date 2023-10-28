using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccessorySaveData
{
    public string ID;
    public string MountKey;

    public AccessoryData GetAccessoryData()
    {
        List<AccessoryData> accessorysData = new(Resources.LoadAll<AccessoryData>("Accessories"));

        foreach (var data in accessorysData)
            if (data.Identifier == ID)
                return data;

        return null;
    }
}
