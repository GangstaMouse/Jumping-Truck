using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IHaveAccessories : MonoBehaviour
{
    List<AccessoryPoint> accessoryPoints = new List<AccessoryPoint>();
}

public struct AccessoryPoint
{
    public Transform MountPoint;
    public string MountKey;
}
