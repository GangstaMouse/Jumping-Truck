using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Accessory")]
public class AccessoryData : ItemDataSO
{
    public string Name;

    public GameObject Prefab;

    public override string ResourceFolderName => "Acc";
}
