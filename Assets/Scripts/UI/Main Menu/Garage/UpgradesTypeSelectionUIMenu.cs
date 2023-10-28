using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradesTypeSelectionUIMenu : BaseSelectionUIMenu<PartSocket>
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    private Vehicle m_VehicleInstance;
    private string carID;

    private GameObject upgradePreviewInstance;
    private GameObject partObject;
    [SerializeField] private UpgradeSelectionUIMenu test;

    protected override void DrawUI()
    {
        m_VehicleInstance = FindObjectOfType<Vehicle>();
        if (m_VehicleInstance == null)
            return;

        List<PartSocket> dataAsset = new(m_VehicleInstance.GetComponentsInChildren<PartSocket>());
        RecreateSelectionList(dataAsset);
        BindSlotsAction(OnSlotSelected);
    }

    // Пофиксить дубликаты моделей, скрывать установленые модели на сокете при предпросмотре
    // Удалить SelectSocketSlot
    // Переименовать в HightlightSlot? ClickedSlot

    protected override void SelectionLogic(PartSocket selectedData)
    {
        test.Initialize(SelectedDataAsset);
        test.Enter();
    }

    protected override void UpdateSelectionUI(PartSocket selectedDataAsset)
    {
    }
}
