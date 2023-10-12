using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Car Data")]
public class CarData : BaseItemData
{
    // Продублировать в качестве ссылок
    public string Name;
    public int Price;
    [SerializeField] private PaintingColorData defaultPaintingAsset;
    public PaintingColorData DefaultPaintingAsset => defaultPaintingAsset;

    public GameObject Prefab;

    public AccessoriesStorage AwaliableAccessories;

    public float Speed = 1f;
    public float Acceleration = 1f;
    public float Suspension = 1f;
    public float Handling = 1f;
    public float Weight = 1f;

    public new const string defaultAssetPath = "Vehicles";

    // Переименовать в GetAssetByID
    /* public static CarData GetCarDataByID(string id)
    {
        foreach (var data in Resources.LoadAll<CarData>("Cars"))
            if (data.ID == id)
                return data;

        return null;
    } */

    public static CarData GetAssetByID(string id, string assetPath = defaultAssetPath) => GetAssetByID<CarData>(id, assetPath);
}

[System.Serializable]
public struct AccessoriesStorage
{
    public AccessoryData AccessoryData;
    public string MountKey;
    // public List<GameObject> Hood;
    // public List<GameObject> Roof;
    // public List<GameObject> RearFenders;
    // public List<GameObject> FrontFenders;
}
