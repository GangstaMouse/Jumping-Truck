using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Car Part")]
public class CarPartData : BaseItemData
{
    public int Price = 500;
    public GameObject ModelPrefab;

    public virtual UpgradeContainer Install(Transform carTransform, Transform attachTo)
    {
        UpgradeContainer container = new UpgradeContainer(carTransform, attachTo);

        if (ModelPrefab != null)
            container.ModelInstance = Instantiate(ModelPrefab, attachTo);

        return container;
    }

    public new const string defaultAssetPath = "Upgrades";

    /* public static CarPartData GetPartDataFromID(string id)
    {
        List<CarPartData> partsData = new List<CarPartData>(Resources.LoadAll<CarPartData>("CarParts"));

        foreach (var data in partsData)
            if (data.ID == id)
                return data;

        return null;
    } */

    public static CarPartData GetAssetByID(string id, string assetPath = defaultAssetPath) => GetAssetByID<CarPartData>(id, assetPath);
}

public class UpgradeContainer
{
    public Transform CarTransform;
    public Transform SocketTransform;
    public GameObject ModelInstance;

    public UpgradeContainer(Transform carTransfrom, Transform socketTransform)
    {
        CarTransform = carTransfrom;
        SocketTransform = socketTransform;
    }

    public virtual void Remove()
    {
        MonoBehaviour.Destroy(ModelInstance);
    }
}
