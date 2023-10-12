using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Accessory")]
public class AccessoryData : BaseItemData
{
    public string Name;
    public int Price;

    public GameObject Prefab;
}
