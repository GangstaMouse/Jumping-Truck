using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Suspension Upgrade")]
public class SuspensionUpgrade : CarPartData
{
    public float AddMaxLenght = 0.1f;

    public override UpgradeContainer Install(Transform carTransform, Transform attachTo)
    {
        SuspensionUpgradeContainer container = new SuspensionUpgradeContainer(carTransform, attachTo);

        if (ModelPrefab != null)
            container.ModelInstance = Instantiate(ModelPrefab, attachTo);

        CarController carController = carTransform.GetComponent<CarController>();
        List<WheelCollider> wheels = new List<WheelCollider>(carController.wheels);

        // Временно
        container.MaxLenght = wheels[0].MaxLenght;

        foreach (var wheel in wheels)
            wheel.MaxLenght += AddMaxLenght;

        return container;
    }
}

public class SuspensionUpgradeContainer : UpgradeContainer
{
    public float MaxLenght;

    public SuspensionUpgradeContainer(Transform carTransfrom, Transform socketTransform) : base(carTransfrom, socketTransform)
    {
    }

    public override void Remove()
    {
        CarController carController = CarTransform.GetComponent<CarController>();
        List<WheelCollider> wheels = new List<WheelCollider>(carController.wheels);

        foreach (var wheel in wheels)
            wheel.MaxLenght = MaxLenght;

        base.Remove();
    }
}
