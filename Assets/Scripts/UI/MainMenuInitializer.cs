using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform spawnPointTransform;

    public static string CarInstanceID = null;
    public static GameObject CarInstanceObject = null;
    public static Transform SpawnPointTransform;

    // Добавить затенение на секунду для более бесшовного спавна машины (скрытие падения при спавне)
    // Хм, в Wreckfest автомобиль типо прогружается при изменении параметров. Думаю это хорошая идея
    private void Awake()
    {
        string carID = PlayerDataProcessor.GetSelectedCarID;
        SpawnPointTransform = spawnPointTransform;

        if (string.IsNullOrEmpty(carID))
        {
            Debug.LogWarning("Car doesn't selected");
            return;
        }

        CarData carData = CarData.GetAssetByID(carID);

        if (carData == null)
            // Лучше перенести в CarData.GetCarDataByID, не имею желанию каждый раз копировать эту строку
            throw new System.Exception($"Car asset with id <<{carID}>> not found");

        CarInstanceID = carID;
        CarInstanceObject = Instantiate(carData.Prefab, spawnPointTransform.position, spawnPointTransform.rotation);

        UICore.LoadPlayerCarTuning(CarInstanceObject, CarInstanceID);
    }

    /* public void RespawnCar()
    {
        CarData carData = CarData.GetCarDataByID(CarInstanceID);
        CarInstanceObject = Instantiate(carData.Prefab, spawnPointTransform.position, spawnPointTransform.rotation);
    } */

    private void OnDestroy()
    {
        CarInstanceID = null;
        CarInstanceObject = null;
        SpawnPointTransform = null;
    }
}
