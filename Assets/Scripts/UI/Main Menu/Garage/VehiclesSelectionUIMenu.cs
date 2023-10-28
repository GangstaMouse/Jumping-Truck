using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VehiclesSelectionUIMenu : BasePurchaseUIMenu<CarDataSO>
{
    [Header("References")]
    [SerializeField] private TMP_Text m_NameText;
    [SerializeField] private TMP_Text m_PriceText;
    [SerializeField] private Button buySelectButton;

    private Vehicle m_VehicleInstance;
    private GameObject m_VehicleInstanceObject;
    private string carID;
    private string m_PreviewCarID;

    [SerializeField] private Transform m_VehicleSpawnPoint;

    // Warning! Меню выбора цвета сделать как в gta5, тоесть заранее заготовленнаую палитру цветов. +Спроектировать систему тюнинга с улучшением производительности

    private CarDataSO highlightedCarData;

    public override void Exit()
    {
        if (m_PreviewCarID != PlayerSavesManager.Vehicle.SelectedCarIdentifier)
            SelectionLogic(ItemDataSO.LoadAssetFromIdentifier<CarDataSO>(PlayerSavesManager.Vehicle.SelectedCarIdentifier, "Vehicles"));

        base.Exit();
    }

    protected override void DrawUI()
    {
        m_PreviewCarID = PlayerSavesManager.Vehicle.SelectedCarIdentifier;
        if (m_VehicleInstance = FindObjectOfType<Vehicle>())
            m_VehicleInstanceObject = m_VehicleInstance.gameObject;

        List<CarDataSO> datasAssets = new(ItemDataSO.LoadAssetsFromPath<CarDataSO>("Vehicles"));
        string selectedPlayerVehicle = PlayerSavesManager.Vehicle.SelectedCarIdentifier;
        System.Func<CarDataSO, bool> func = dataAsset => string.IsNullOrEmpty(selectedPlayerVehicle) == false &&
            dataAsset.Identifier == selectedPlayerVehicle;
        RecreateSelectionList(datasAssets, func, true);
        BindSlotsAction(OnSlotSelected);
    }

    protected override void SelectionLogic(CarDataSO selectedData)
    {
        if (selectedData.Identifier == m_PreviewCarID)
            return;

        m_PreviewCarID = selectedData.Identifier;

        Destroy(m_VehicleInstanceObject);
        m_VehicleInstanceObject = Instantiate(selectedData.Prefab, m_VehicleSpawnPoint);
        InitializeVehicle(m_VehicleInstanceObject, selectedData);
    }

    private void InitializeVehicle(GameObject vehicleInstanceObject, CarDataSO vehicleDataAsset)
    {
        PlayerSavesManager.Vehicle.Upgrades.Utils.PaintCar(m_VehicleInstanceObject, vehicleDataAsset.DefaultPaintAsset.Color);

        if (vehicleDataAsset.Identifier == PlayerSavesManager.Vehicle.SelectedCarIdentifier)
            PlayerSavesManager.Vehicle.Upgrades.Utils.LoadPlayerCarTuning(m_VehicleInstanceObject, PlayerSavesManager.Vehicle.SelectedCarIdentifier);
    }

    protected override void UpdateSelectionUI(CarDataSO selectedDataAsset)
    {
        m_NameText.SetText(selectedDataAsset.Name);

        // Not owned
        // Owned not selected
        // Owned selected

        bool priceEnabled;
        string buttonText;

        if (PlayerSavesManager.Vehicle.DoesVehicleOwned(selectedDataAsset))
        {
            priceEnabled = false;
            buttonText = "Owned";

            if (selectedDataAsset.Identifier == PlayerSavesManager.Vehicle.SelectedCarIdentifier)
            {
                buttonText = "Selected";
            }
        }
        else
        {
            priceEnabled = true;
            buttonText = "Purchase";

            m_PriceText.SetText($"Price {selectedDataAsset.Price}");
        }

        m_PriceText.gameObject.SetActive(priceEnabled);
        ActionText.SetText(buttonText);
    }

    [System.Obsolete]
    private void Test()
    {
        Vehicle carController = m_VehicleInstance.GetComponent<Vehicle>();
        GetComponentInChildren<CarInfoUI>().UpdateInfo(carController);
    }

    [System.Obsolete]
    private void ReceiveAndSelectDefaultUpgrades()
    {
        List<PartSocket> sockets = new(m_VehicleInstance.GetComponentsInChildren<PartSocket>());

        foreach (var socket in sockets)
            if (socket.InstalledPart != null)
            {
                PlayerSavesManager.Vehicle.Upgrades.AddVehiclePart(socket.InstalledPart);
                PlayerSavesManager.Vehicle.Upgrades.SelectVehiclePart(carID, socket.ID, socket.InstalledPart);
            }
    }

    protected override void OnSelectPurchaseButton(CarDataSO dataAsset)
    {
        if (dataAsset == null || string.IsNullOrEmpty(dataAsset.Identifier) || dataAsset.Identifier == PlayerSavesManager.Vehicle.SelectedCarIdentifier)
            return;

        if (PlayerSavesManager.Vehicle.DoesVehicleOwned(dataAsset))
            PlayerSavesManager.Vehicle.SelectVehicle(dataAsset);
        else
            PlayerSavesManager.Vehicle.PurchaseVehicle(dataAsset);

        PlayerSavesManager.Save();
        UpdateSelectionUI(dataAsset);
    }
}
