using UnityEngine;

[CreateAssetMenu(fileName = "New CarData", menuName = "Resources/Car")]
public sealed class CarDataSO : OwnableItemDataSO
{
    public string Name;
    [field: SerializeField] public PaintDataSO DefaultPaintAsset { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }

    // public AccessoriesStorage AvaliableAccessories;
    public override string ResourceFolderName => "Vehicles";

    public float Speed = 1f;
    public float Acceleration = 1f;
    public float Suspension = 1f;
    public float Handling = 1f;
    public float Weight = 1f;
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
