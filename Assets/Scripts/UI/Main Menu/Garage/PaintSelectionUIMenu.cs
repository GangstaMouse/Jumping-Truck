using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaintSelectionUIMenu : BasePurchaseUIMenu<PaintDataSO>
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button backButton;

    private Vehicle m_VehicleInstance;
    private string carID;

    private List<Material> m_VehicleMaterials = new();
    private Color m_VehicleOriginalColor;

    protected override void DrawUI()
    {
        m_VehicleInstance = FindObjectOfType<Vehicle>();
        if (m_VehicleInstance == null)
            return;

        m_VehicleMaterials = FunctionsLibrary.GetListOfMaterialsByName(m_VehicleInstance.gameObject, "Car Paint");
        m_VehicleOriginalColor = FunctionsLibrary.GetMaterialsMostColor(m_VehicleMaterials);

        List<PaintDataSO> datasAssets = new(ItemDataSO.LoadAssetsFromPath<PaintDataSO>("Paints"));
        datasAssets.Sort(SortColorByHSV);
        string selectedHexColor = PlayerSavesManager.Vehicle.GetCarPaintHexColor(PlayerSavesManager.Vehicle.SelectedCarIdentifier);
        System.Func<PaintDataSO, bool> func = dataAsset => selectedHexColor == ColorToHex(dataAsset.Color);
        RecreateSelectionList(datasAssets,func);
        BindSlotsAction(OnSlotSelected);
    }

    public override void Exit()
    {
        PaintCar(m_VehicleOriginalColor);
        base.Exit();
    }

    // Изменить, добавлять # только при приминении цвета, при сохранении убирать
    // Так! HexColor уже так сохраняется, без решётки
    private string ColorToHex(in Color color) => ColorUtility.ToHtmlStringRGB(color);

    // Написано ооочень плохо
    // Инвертинованна
    private int SortColorByHSV(PaintDataSO a, PaintDataSO b)
    {
        Color.RGBToHSV(a.Color, out float hA, out float sA, out float vA);
        Color.RGBToHSV(b.Color, out float hB, out float sB, out float vB);

        if (hA < hB)
            return 1;
        else if (hA > hB)
            return -1;
        else
        {
            if (sA < sB)
                return 1;
            else if (sA > sB)
                return -1;
            else
            {
                if (vA < vB)
                    return 1;
                else if (vA > vB)
                    return -1;
            }
        }

        return 0;
    }

    private string GetClosestPaintID()
    {
        List<PaintDataSO> paintingColorDatas = new(Resources.LoadAll<PaintDataSO>("Colors"));

        string paintingID = null;
        float colorValue = 3f;

        foreach (var painting in paintingColorDatas)
        {
            float v = GetDifference(painting.Color, m_VehicleOriginalColor);

            if (v < colorValue)
            {
                paintingID = painting.Identifier;
                colorValue = v;
            }
        }

        return paintingID;
    }

    private float GetDifference(Color colorA, Color colorB)
    {
        float r = colorA.r - colorB.r;
        float g = colorA.g - colorB.g;
        float b = colorA.b - colorB.b;

        return r * r + g * g + b * b;
    }

    private void PaintCar(Color paintColor)
    {
        foreach (var material in m_VehicleMaterials)
            material.color = paintColor;
    }

    protected override void OnSelectPurchaseButton(PaintDataSO dataAsset)
    {
        string hexColor = ColorToHex(dataAsset.Color);
        if (hexColor == PlayerSavesManager.Vehicle.GetCarPaintHexColor(PlayerSavesManager.Vehicle.SelectedCarIdentifier))
            return;

        if (PlayerSavesManager.Vehicle.PurchasePaint(PlayerSavesManager.Vehicle.SelectedCarIdentifier, dataAsset))
        {
            PlayerSavesManager.Save();
            m_VehicleOriginalColor = dataAsset.Color;
        }
    }

    protected override void SelectionLogic(PaintDataSO selectedData)
    {
        PaintCar(selectedData.Color);
    }

    protected override void UpdateSelectionUI(PaintDataSO selectedDataAsset)
    {
        nameText.SetText(selectedDataAsset.name);

        // Not painted
        // Painted

        bool priceEnabled;
        string buttonText; 

        string hexColor = ColorToHex(selectedDataAsset.Color);
        
        if (hexColor == PlayerSavesManager.Vehicle.GetCarPaintHexColor(carID))
        {
            priceEnabled = false;
            buttonText = "Painted";
        }
        else
        {
            priceEnabled = true;
            buttonText = "Purchase";
            priceText.SetText($"Price {selectedDataAsset.Price}");
        }

        priceText.gameObject.SetActive(priceEnabled);
        ActionText.SetText(buttonText);
    }
}
