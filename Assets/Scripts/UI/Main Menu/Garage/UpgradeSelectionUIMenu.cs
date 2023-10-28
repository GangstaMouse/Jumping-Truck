using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectionUIMenu : BasePurchaseUIMenu<BasePart>
{
    private PartSocket m_SelectedSocket;
    private BasePart m_OriginalPart;
    private BasePart m_PreviewPart;

    public void Initialize(in PartSocket selectedSocket)
    {
        m_SelectedSocket = selectedSocket;
        m_OriginalPart = m_SelectedSocket.InstalledPart;
    }

    protected override void ExitFromSubMenu()
    {
        if (m_SelectedSocket.InstalledPart != m_OriginalPart)
            InstallPart(m_OriginalPart);

        base.ExitFromSubMenu();
    }

    protected override void DrawUI()
    {
        ClearUI();
        List<BasePart> datasAssets = new(m_SelectedSocket.AvailableParts);
        if (m_SelectedSocket.Nullable)
            datasAssets.Insert(0, null);

        var selectedUpgradeID = PlayerSavesManager.Vehicle.Upgrades.GetInstalledPartID(PlayerSavesManager.Vehicle.SelectedCarIdentifier, m_SelectedSocket.ID);
        Func<BasePart, bool> func = dataAsset => string.IsNullOrEmpty(selectedUpgradeID) == false &&
            dataAsset.Identifier == selectedUpgradeID;
        RecreateSelectionList(datasAssets, func, m_SelectedSocket.Nullable);
        BindSlotsAction(OnSlotSelected);
    }

    private void InstallPart(BasePart partData)
    {
        m_SelectedSocket.Install(partData);
    }

    private bool DoesPartUnlocked(BasePart partData)
    {
        if (string.IsNullOrEmpty(partData.UnlockRequirements))
            return true;

        bool unlocked = true;

        // Удаление пробелов, и разделение по запятой
        List<string> requirements = new(partData.UnlockRequirements.Replace(" ", null).Split(','));

        foreach (var requirement in requirements)
        {
            string condition;
            string value;

            FunctionsLibrary.GetValuesFromCommand(requirement, out condition, out value);

            // Добавить условие ИЛИ
            switch (condition)
            {
                case "level":
                    if (PlayerSavesManager.Level < int.Parse(value))
                        return false;
                    break;

                // Добавить реализацию в будущем
                case "ads":
                    if (1 < int.Parse(value))
                        return false;
                    break;
            }
        }

        return unlocked;
    }

    protected override void OnSelectPurchaseButton(BasePart dataAsset)
    {
        if (!PlayerSavesManager.Vehicle.Upgrades.DoesPartReceived(dataAsset))
            return;

        if (DoesPartUnlocked(dataAsset) && PlayerSavesManager.Vehicle.Upgrades.PurchaseVehiclePart(dataAsset))
        {
            PlayerSavesManager.Vehicle.Upgrades.SelectVehiclePart(PlayerSavesManager.Vehicle.SelectedCarIdentifier, SelectedDataAsset.Identifier, dataAsset);
            PlayerSavesManager.Save();
            m_OriginalPart = m_PreviewPart;
        }
    }

    protected override void SelectionLogic(BasePart selectedData)
    {
        if (selectedData != m_PreviewPart)
        {
            InstallPart(selectedData);
            m_PreviewPart = selectedData;
        }
    }

    protected override void UpdateSelectionUI(BasePart selectedDataAsset)
    {
        // Not owned
        // Owned not installed
        // Owned Installed
    }
}
