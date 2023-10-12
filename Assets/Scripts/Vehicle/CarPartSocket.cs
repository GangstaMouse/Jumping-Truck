using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPartSocket : MonoBehaviour
{
    // [SerializeField] private string socketName = "Default";
    public string Name;
    // [SerializeField] private CarPartData upgradeData;
    // Думаю можно инкапсулировать если назначать через InstallUpgrade. + Не давать в старте устанавливать новую деталь если уже имеется контейнер
    public CarPartData PartData;
    // [SerializeField] private string id;
    public string ID;
    // Пока пожалуй повременю с инкапсуляцией

    // Совсем забыл что ранее была идея с добавление оффсета, и дополнительного вращения деталей для лучшего позиционирования универсальных деталей
    public List<CarPartData> AvailableUpgrades = new List<CarPartData>();

    private Transform carTransform;
    private UpgradeContainer container;

    private void Awake() => carTransform = transform.GetComponentInParent<CarController>().transform;

    private void Start() => InstallUpgrade(PartData);

    public void InstallUpgrade(CarPartData upgradeData)
    {
        if (container != null)
        {
            container.Remove();
            container = null;
        }

        if (upgradeData == null)
            return;

        container = upgradeData.Install(carTransform, transform);
    }
}
