using System.Collections.Generic;
using UnityEngine;

public class PartSocket : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; } = "Name";
    [field: SerializeField] public List<BasePart> AvailableParts { get; private set; } = new();
    [field: SerializeField] public BasePart InstalledPart { get; private set; }
    [field: SerializeField] public string ID { get; private set; }
    public PartInstance PartInstance { get; private set; }
    [field: SerializeField] public bool Nullable { get; private set; } = true;

    private void Start()
    {
        if (InstalledPart == null && Nullable == false && AvailableParts.Count != 0)
            InstalledPart = AvailableParts[0];

        if (InstalledPart != null)
        {
            Install(InstalledPart);
        }
    }

    public void Install(BasePart partToInstall)
    {
        if (PartInstance != null)
        {
            PartInstance.Remove();
            PartInstance = null;
        }

        if (partToInstall != null)
            PartInstance = partToInstall.CreatePartInstance(transform);
    }
}
