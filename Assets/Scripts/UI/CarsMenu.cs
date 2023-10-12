using System;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class CarsMenu : MonoBehaviour
{
    // Удалить
    [Header("Settings")]
    [SerializeField] private float cartSpeed = 2f;

    [Header("Prefab")]
    [SerializeField] private GameObject slotPrefab;

    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private RectTransform contentList;
    [SerializeField] private Button buySelectButton;
    // Удалить
    [SerializeField] private CinemachineDollyCart cart;

    private GameObject carObject;
    private string carID;

    // Теперь это неверно
    public event Action<CarData> OnSlotHighlighted;

    private bool menuVisibility;

    // Warning! Меню выбора цвета сделать как в gta5, тоесть заранее заготовленнаую палитру цветов. +Спроектировать систему тюнинга с улучшением производительности
    private BaseItemSlot highlightedSlot;
    private BaseItemSlot selectedSlot;

    private CarData highlightedCarData;

    public void ToggleVisibility() => SetVisibility(!menuVisibility);

    public void SetVisibility(bool newVisibility)
    {
        menuVisibility = newVisibility;

        gameObject.SetActive(menuVisibility);

        if (menuVisibility)
            RecreateListOfCars();
        else
            UICore.ClearContent(contentList);

        cart.m_Speed = (menuVisibility ? cartSpeed : -cartSpeed);
    }

    /* private void OnEnable()
    {
        PlayerDataProcessor.OnCashValueChanged += RefreshCashText;
        RefreshCashText(PlayerDataProcessor.GetCash());
    } */

    // private void OnDisable() => PlayerDataProcessor.OnCashValueChanged -= RefreshCashText;

    // private void RefreshCashText(int cashValue) => cashText.SetText($"Cash: {cashValue}");

    public void Enter()
    {
        carObject = MainMenuInitializer.CarInstanceObject;
        carID = MainMenuInitializer.CarInstanceID;

        gameObject.SetActive(true);
        RecreateListOfCars();
    }

    public void Exit()
    {
        gameObject.SetActive(false);

        /* if (!PlayerDataProcessor.DoesCarPurchased(highlightedCarData))
        {
            Destroy(carObject);
            CarData carData = CarData.GetCarDataByID(MainMenuInitializer.CarInstanceID);
            // MainMenuInitializer.CarInstanceObject = Instantiate(carda)
        } */

        if (carID != MainMenuInitializer.CarInstanceID)
        {
            Destroy(carObject);

            if (!string.IsNullOrEmpty(MainMenuInitializer.CarInstanceID))
            {
                CarData carData = CarData.GetAssetByID(MainMenuInitializer.CarInstanceID);
                Transform spawnPointTransform = MainMenuInitializer.SpawnPointTransform;
                MainMenuInitializer.CarInstanceObject = Instantiate(carData.Prefab, spawnPointTransform.position, spawnPointTransform.rotation);
                UICore.LoadPlayerCarTuning(MainMenuInitializer.CarInstanceObject, MainMenuInitializer.CarInstanceID);
            }
        }

        /* carObject = null;
        carID = null; */

        highlightedSlot = null;
        selectedSlot = null;

        highlightedCarData = null;
    }

    private void RecreateListOfCars()
    {
        UICore.ClearContent(contentList);

        List<CarData> carDatas = new List<CarData>(Resources.LoadAll<CarData>(CarData.defaultAssetPath));

        foreach (var data in carDatas)
        {
            GameObject slotInstanceObject = Instantiate(slotPrefab, contentList);
            BaseItemSlot slot = slotInstanceObject.GetComponent<BaseItemSlot>();
            slot.Initialize(data, ClickedSlot);

            // Debug.LogWarning(carID == data.ID);

            if (highlightedCarData == null)
            {
                if (!string.IsNullOrEmpty(carID))
                {
                    if (data.ID == carID)
                    {
                        highlightedCarData = data;
                        ClickedSlot(slot, data);
                        // Перенести, так как в нижнем случае оно не будет срабатывать
                        SelectSlot(slot);
                        Test();
                    }
                }
                else
                    ClickedSlot(slot, data);
            }
            else if (highlightedSlot == null)
            {
                ClickedSlot(slot, data);
            }
        }
    }

    private void ClickedSlot(BaseItemSlot slot, object data)
    {
        if (slot == highlightedSlot)
            return;

        HighlightSlot((CarSlot)slot);

        CarData carData = (CarData)data;

        UpdateUI(carData);

        // if (carData.ID == carID)
        if (carData == highlightedCarData)
            return;

        highlightedCarData = carData;

        // Respawn new car
        Destroy(carObject);
        Transform spawnPointTransform = MainMenuInitializer.SpawnPointTransform;
        carObject = Instantiate(carData.Prefab, spawnPointTransform.position, spawnPointTransform.rotation);
        carID = carData.ID;

        if (PlayerDataProcessor.DoesCarPurchased(carData))
        {
            UICore.LoadPlayerCarTuning(carObject, carID);
            MainMenuInitializer.CarInstanceObject = carObject;
            MainMenuInitializer.CarInstanceID = carID;
        }
        else if (carData.DefaultPaintingAsset != null)
            UICore.PaintCar(carObject, carData.DefaultPaintingAsset.Color);

        Test();
    }

    [System.Obsolete]
    private void Test()
    {
        CarController carController = carObject.GetComponent<CarController>();
        GetComponentInChildren<CarInfoUI>().UpdateInfo(carController);
    }

    private void HighlightSlot(BaseItemSlot slot)
    {
        if (highlightedSlot != null)
            highlightedSlot.Deselect();

        highlightedSlot = slot;
        highlightedSlot.Select();

        OnSlotHighlighted?.Invoke(highlightedCarData);
    }

    private void SelectSlot(BaseItemSlot slot)
    {
        if (selectedSlot != null)
            selectedSlot.Highlight(false);

        selectedSlot = slot;
        selectedSlot.Highlight(true);
    }

    private void UpdateUI(CarData carData)
    {
        bool priceEnabled = true;
        string buttonText = "Buy";
        nameText.SetText(carData.Name);
        UnityAction buttonAction = BuyCar;

        if (PlayerDataProcessor.DoesThisCarSelected(carData))
        {
            priceEnabled = false;
            buttonText = "Selected";
            buttonAction = null;
        }
        else if (PlayerDataProcessor.DoesCarPurchased(carData))
        {
            priceEnabled = false;
            buttonText = "Select";
            buttonAction = SelectCar;
        }

        if (priceEnabled)
            priceText.SetText($"Price {carData.Price}");

        priceText.enabled = priceEnabled;

        buySelectButton.onClick.RemoveAllListeners();
        buySelectButton.onClick.AddListener(buttonAction);
        buySelectButton.GetComponentInChildren<TMP_Text>().SetText(buttonText);
    }

    public void ActionButton()
    {

    }

    public void BuyCar()
    {
        if (highlightedCarData == null)
            return;

        if (!PlayerDataProcessor.BuyCar(highlightedCarData))
            return;

        ReceiveAndSelectDefaultUpgrades();

        // PlayerDataProcessor.Save();

        MainMenuInitializer.CarInstanceID = highlightedCarData.ID;
        MainMenuInitializer.CarInstanceObject = carObject;

        UpdateUI(highlightedCarData);
        // UpdateUI();
    }

    private void ReceiveAndSelectDefaultUpgrades()
    {
        List<CarPartSocket> sockets = new List<CarPartSocket>(carObject.GetComponentsInChildren<CarPartSocket>());

        foreach (var socket in sockets)
            if (socket.PartData != null)
            {
                PlayerDataProcessor.GiveCarPart(socket.PartData);
                PlayerDataProcessor.SetCarPartData(carID, socket.ID, socket.PartData);
            }
    }

    private void SelectCar()
    {
        if (!PlayerDataProcessor.DoesCarPurchased(highlightedCarData))
            return;

        // Добавить сохранение
        PlayerDataProcessor.SetSelectedCar(highlightedCarData);

        // PlayerDataProcessor.Save();

        SelectSlot(highlightedSlot);
        UpdateUI(highlightedCarData);
    }
}
