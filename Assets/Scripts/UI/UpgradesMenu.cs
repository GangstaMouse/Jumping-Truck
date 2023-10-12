using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UpgradesMenu : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject socketSlotPrefab;
    [SerializeField] private GameObject upgradeSlotPrefab;

    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private RectTransform contentList;
    [SerializeField] private Button actionButton;

    [SerializeField] private UnityEvent onBackButton;

    private GameObject carObject;
    private string carID;

    private BaseItemSlot highlightedSlot;
    private BaseItemSlot selectedSlot;

    private CarPartSocket selectedSocket;
    private CarPartData highlightedPartData;

    private GameObject upgradePreviewInstance;
    private GameObject partObject;

    public void Enter()
    {
        carObject = MainMenuInitializer.CarInstanceObject;
        carID = MainMenuInitializer.CarInstanceID;

        gameObject.SetActive(true);

        RecreateListOfUpgrades();
    }
    
    public void BackButton()
    {
        // Go to Sokets selection list
        if (selectedSocket != null)
        {
            string installedPartID = PlayerDataProcessor.GetInstalledPartID(carID, selectedSocket.ID);
            CarPartData installedUpgradeData = CarPartData.GetAssetByID(installedPartID);

            if (highlightedPartData != installedUpgradeData)
                selectedSocket.InstallUpgrade(installedUpgradeData);

            selectedSocket = null;
            RecreateListOfUpgrades();
        }
        // Closing menu
        else
        {
            gameObject.SetActive(false);
            onBackButton?.Invoke();
        }
    }

    public void RecreateListOfUpgrades()
    {
        List<CarPartSocket> partSockets = new List<CarPartSocket>(carObject.GetComponentsInChildren<CarPartSocket>());

        UICore.ClearContent(contentList);
        // selectedSocket = null;
        highlightedSlot = null;
        highlightedPartData = null;
        priceText.enabled = false;
        Destroy(upgradePreviewInstance);

        actionButton.gameObject.SetActive(false);
        priceText.enabled = false;
        nameText.enabled = false;

        // Activate installed part
        /* if (selectedSocket != null && selectedSocket.partModelInstance != null)
            selectedSocket.partModelInstance.SetActive(true); */

        foreach (var socket in partSockets)
        {
            GameObject slotObject = Instantiate(socketSlotPrefab, contentList);
            BaseItemSlot slot = slotObject.GetComponent<BaseItemSlot>();
            slot.Initialize(socket, SelectSocketSlot);
        }
    }

    // Можно объединить с SelectPartSlot
    private void SelectSocketSlot(BaseItemSlot slot, object data)
    {
        // selectedSlot = (SocketSlot)slot;
        selectedSocket = (CarPartSocket)data;

        RecreateListOfParts();
    }

    public void RecreateListOfParts()
    {
        List<CarPartData> partDatas = new List<CarPartData>(selectedSocket.AvailableUpgrades);

        UICore.ClearContent(contentList);
        highlightedSlot = null;
        highlightedPartData = null;
        priceText.enabled = true;

        string installedPartID = PlayerDataProcessor.GetInstalledPartID(carID, selectedSocket.ID);
        CarPartData installedUpgradeData = CarPartData.GetAssetByID(installedPartID);

        foreach (var data in partDatas)
        {
            GameObject slotObject = Instantiate(upgradeSlotPrefab, contentList);
            BaseItemSlot slot = slotObject.GetComponent<BaseItemSlot>();
            slot.Initialize(data, SelectPartSlot);

            if (selectedSlot == null)
            {
                if (installedUpgradeData == null)
                {
                    // Думаю от этого можно избавиться, так как теперь при получении авто эти детали тоже получаются, и выбираются
                    /* if (selectedSocket.PartData != null && selectedSocket.PartData == data)
                    {
                        highlightedPartData = data;
                        SelectPartSlot(slot, data);
                        SelectSlot(slot);
                    }
                    else */ if (selectedSocket.PartData == null)
                    {
                        SelectPartSlot(slot, data);
                        SelectSlot(slot);
                    }
                }
                else if (installedUpgradeData == data)
                {
                    highlightedPartData = data;
                    SelectPartSlot(slot, data);
                    SelectSlot(slot);
                }
            }

            // --- Prev

            /* if (highlightedPartData == null)
            {
                if (!string.IsNullOrEmpty(installedPartID) && data.ID == installedPartID)
                {
                    highlightedPartData = data;
                    SelectPartSlot(slot, data);
                    SelectSlot(slot);
                }
                else
                    UpdateUI(null);
            } */
        }
    }

    // Пофиксить дубликаты моделей, скрывать установленые модели на сокете при предпросмотре
    // Удалить SelectSocketSlot
    // Переименовать в HightlightSlot? ClickedSlot
    private void SelectPartSlot(BaseItemSlot slot, object data)
    {
        /* if (data is CarPartSocket)
        {
            selectedSocket = (CarPartSocket)data;
            RecreateListOfParts();
            return;
        } */

        if (slot == highlightedSlot)
            return;

        HightlightSlot(slot);

        CarPartData partData = (CarPartData)data;
        UpdateUI(partData);

        if (partData == highlightedPartData)
            return;

        highlightedPartData = partData;

        selectedSocket.InstallUpgrade(partData);

        return;

        UpgradePreview(partData);

        ChangeActionButton(partData);

        // ----<Separator

        /* bool unlocked = DoesPartUnlocked(partData);

        if (PlayerDataProcessor.DoesPartReceived(partData))
            priceText.SetText("Already received");
        else if (unlocked)
            priceText.SetText($"Price {partData.Price}");
        else
            priceText.SetText("Locked");

        Debug.LogWarning(unlocked);
        if (!unlocked)
            return; */

        /* selectedSocket.PartData = partData;
        selectedSocket.InstallPart();

        PlayerDataProcessor.SetCarPartData(carInstanceData.ID, selectedSocket.ID, partData);
        PlayerDataProcessor.Save(); */
    }

    private void UpgradePreview(CarPartData partData)
    {
        Destroy(upgradePreviewInstance);
        
        upgradePreviewInstance = Instantiate(partData.ModelPrefab, selectedSocket.transform);
    }

    private bool DoesPartUnlocked(CarPartData partData)
    {
        if (string.IsNullOrEmpty(partData.UnlockRequirements))
            return true;

        bool unlocked = true;

        // Удаление пробелов, и разделение по запятой
        List<string> requirements = new List<string>(partData.UnlockRequirements.Replace(" ", null).Split(','));

        foreach (var requirement in requirements)
        {
            string condition;
            string value;

            FunctionsLibrary.GetValuesFromCommand(requirement, out condition, out value);

            // Добавить условие ИЛИ
            switch (condition)
            {
                case "level":
                    if (PlayerDataProcessor.GetLevelValue < int.Parse(value))
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

    // Исправить. Функции подсветки и выделения работают наоборот
    private void HightlightSlot(BaseItemSlot slot)
    {
        if (highlightedSlot != null)
            highlightedSlot.Deselect();

        highlightedSlot = slot;
        highlightedSlot.Select();
    }

    private void SelectSlot(BaseItemSlot slot)
    {
        if (selectedSlot != null)
            selectedSlot.Highlight(false);

        selectedSlot = slot;
        selectedSlot.Highlight(true);
    }
    // ---

    private void ChangeActionButton(CarPartData partData)
    {
        TMP_Text buttonText = actionButton.GetComponentInChildren<TMP_Text>();
        UnityAction buttonAction = null;
        string actionText = null;
        bool buttonEnabled = true;

        bool unlocked = DoesPartUnlocked(partData);

        if (PlayerDataProcessor.DoesPartReceived(partData))
        {
            priceText.SetText("Already received");

            if (PlayerDataProcessor.GetInstalledPartID(carID, selectedSocket.ID) != partData.ID)
            {
                actionText = "Install";
                buttonAction = InstallPart;
            }
            else
                buttonEnabled = false;
        }
        else if (unlocked)
        {
            priceText.SetText($"Price {partData.Price}");
            actionText = "Buy";
            buttonAction = BuyPart;
        }
        else
        {
            priceText.SetText("Locked");
            // actionText = "Need Unlock";
            buttonEnabled = false;
        }

        actionButton.gameObject.SetActive(buttonEnabled);
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(buttonAction);
        buttonText.SetText(actionText);
    }

    // Поправить
    private void UpdateUI(CarPartData partData)
    {
        string buttonText = "Purchase";
        actionButton.gameObject.SetActive(true);

        if (partData == null)
        {
            nameText.enabled = false;
            priceText.enabled = false;
            actionButton.gameObject.SetActive(false);
            return;
        }

        if (PlayerDataProcessor.DoesPartReceived(partData))
        {
            priceText.enabled = false;
            buttonText = "Purchased";
        }
        else
        {
            priceText.enabled = true;
            priceText.SetText($"Price: {partData.Price}");
        }

        nameText.SetText(partData.name);
        actionButton.GetComponentInChildren<TMP_Text>().SetText(buttonText);
    }

    public void ActionButton()
    {
        if (highlightedSlot == null)
            return;

        CarPartData partData = (CarPartData)highlightedPartData;

        // Buy part
        if (!PlayerDataProcessor.DoesPartReceived(partData))
            if (!PlayerDataProcessor.BuyCarPart(partData))
                return;
        // Install part
        else
        {
            PlayerDataProcessor.SetCarPartData(carID, selectedSocket.ID, partData);
            SelectSlot(highlightedSlot);
        }

        ChangeActionButton(partData);
    }

    private void BuyPart()
    {
        CarPartData partData = (CarPartData)highlightedPartData;

        if (PlayerDataProcessor.DoesPartReceived(partData))
            return;

        if (!PlayerDataProcessor.BuyCarPart(partData))
            return;

        // PlayerDataProcessor.Save();

        ChangeActionButton(partData);
    }

    private void InstallPart()
    {
        CarPartData partData = (CarPartData)highlightedPartData;

        if (!PlayerDataProcessor.DoesPartReceived(partData))
            return;

        PlayerDataProcessor.SetCarPartData(carID, selectedSocket.ID, partData);

        // PlayerDataProcessor.Save();

        SelectSlot(highlightedSlot);
        ChangeActionButton(partData);
        // Добавить socket.part = , socket.Install
    }
}
