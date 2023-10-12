using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Переименовать в PlayerCarSpawn
public class CarSpawn : MonoBehaviour
{
    [SerializeField] private GameObject mobileControlsUIPrefab;

    // private static GameObject carPrefab => PlayerDataProcessor.GetSelectedCarData().Prefab;
    private CarData carData;
    private GameObject carPrefab;
    private GameObject uiPrefab;

    private GameObject carInstance;
    // private GameObject uiInstance;
    private GameObject mobileUIControlsInstance;

    public void Initialize(GameObject carPrefab, GameObject uiPrefab)
    {
        this.carPrefab = carPrefab;
        this.uiPrefab = uiPrefab;
    }

    // public static void SetCarPrefab(GameObject prefab) => carPrefab = prefab;

    private void Start()
    {
        carData = PlayerDataProcessor.GetSelectedCarData();
        carPrefab = carData.Prefab;
        SpawnCar();

        if (Application.isMobilePlatform || Application.isEditor)
            mobileUIControlsInstance = Instantiate(mobileControlsUIPrefab);
    }

    // Переделать
    public void SpawnCar()
    {
        if (carPrefab == null)
            return;

        carInstance = Instantiate(carPrefab, transform.position, transform.rotation);

        CarController carController = carInstance.GetComponent<CarController>();
        InitializeCarPartSockets();
        // Добавить применение выбранного цвета автомобилю
        LoadColor();

        // Controller activation
        // carController.SetController(Controller.Player);
        carController.controller = Controller.Player;
        carController.InputEnabled = true;
    }

    private void InitializeCarPartSockets()
    {
        List<CarPartSocket> partSockets = new List<CarPartSocket>(carInstance.GetComponentsInChildren<CarPartSocket>());

        foreach (var socket in partSockets)
        {
            string partID = PlayerDataProcessor.GetInstalledPartID(carData.ID, socket.ID);
            Debug.Log(partID);
            CarPartData partData = CarPartData.GetAssetByID(partID);
            Debug.Log(partData);

            if (partData != null)
                socket.PartData = partData;

            /* CarPartData partData = PlayerDataProcessor.GetCarPartData(carData.ID, socket.ID);
            Debug.Log(socket.ID);
            if (partData != null)
                socket.PartData = partData; */
        }
    }
    
    [System.Obsolete]
    private void LoadColor()
    {
        // Load Paint Color
        string hexColor = PlayerDataProcessor.GetCarPaintHexColor(carData.ID);
        Color paintColor;

        if (ColorUtility.TryParseHtmlString(hexColor, out paintColor))
        // if (ColorUtility.TryParseHtmlString($"#{hexColor}", out paintColor))
            PaintCar(carInstance, paintColor);
        else
            Debug.LogWarning($"HexColor <<{hexColor}>> is invalid");
    }

    [System.Obsolete]
    public static void PaintCar(GameObject carObject, Color paintColor)
    {
        List<Material> materials = new List<Material>(FunctionsLibrary.GetListOfMaterialsByName(carObject, "Car Paint"));

        foreach (var material in materials)
            material.color = paintColor;
    }
}
