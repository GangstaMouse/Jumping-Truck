using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IHaveAccessories : MonoBehaviour
{
    List<AccessoryPoint> accessoryPoints = new();
}

public struct AccessoryPoint
{
    public Transform MountPoint;
    public string MountKey;
}
