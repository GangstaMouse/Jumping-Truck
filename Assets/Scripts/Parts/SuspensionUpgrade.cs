using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Suspension Upgrade")]
public class SuspensionUpgrade : BasePart
{
    public float AddMaxLenght = 0.1f;

    public override PartInstance CreatePartInstance(Transform socketTransform)
    {
        SuspensionPartInstance container = new(this, socketTransform);

        List<CustomWheelCollider> wheels = new(socketTransform.GetComponentInParent<Vehicle>().WheelColliders);

        // Временно
        container.MaxLenght = wheels[0].MaxSuspensionLenght;

        // foreach (var wheel in wheels)
            // wheel.MaxSuspensionLenght += AddMaxLenght;

        return container;
    }
}

public class SuspensionPartInstance : PartInstance
{
    public float MaxLenght;
    public Vehicle Vehicle;

    public SuspensionPartInstance(BasePart part, Transform socketTransform) : base(part, socketTransform)
    {
    }

    public override void Remove()
    {
        // List<CustomWheelCollider> wheels = new(Vehicle.WheelColliders);

        /* foreach (var wheel in wheels)
            wheel.MaxSuspensionLenght = MaxLenght; */

        base.Remove();
    }
}
