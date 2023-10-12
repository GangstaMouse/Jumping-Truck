using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PaintingMenu : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject slotPrefab;

    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private RectTransform contentList;
    [SerializeField] private Button actionButton;
    [SerializeField] private Button backButton;

    private GameObject carObject;
    private string carID;

    private List<Material> materials = new List<Material>();
    private Color originalColor;

    private BaseItemSlot highlightedSlot;
    private BaseItemSlot selectedSlot;

    private PaintingColorData highlightedPaintData;
    private PaintingColorData selectedColorData;

    public void SetVisibility(bool newVisibility)
    {
        gameObject.SetActive(newVisibility);

        /* if (newVisibility)
            RecreateListOfColors(); */
    }

    // Изменить, добавлять # только при приминении цвета, при сохранении убирать
    // Так! HexColor уже так сохраняется, без решётки
    [System.Obsolete]
    private string GetHexColor(Color color) => $"#{ColorUtility.ToHtmlStringRGB(color)}";

    public void Enter()
    {
        carObject = MainMenuInitializer.CarInstanceObject;
        carID = MainMenuInitializer.CarInstanceID;

        gameObject.SetActive(true);
        materials = new List<Material>(FunctionsLibrary.GetListOfMaterialsByName(carObject, "Car Paint"));
        originalColor = FunctionsLibrary.GetMaterialsMostColor(materials);

        /* actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(BuyPaint); */

        RecreateListOfColors();

        // backButton.onClick.AddListener(Exit);
    }

    public void Exit()
    {
        gameObject.SetActive(false);

        // Попровать
        if (highlightedPaintData != null)
        {
            // string hexColor = ColorUtility.ToHtmlStringRGB(highlightedPaintData.Color);
            string hexColor = GetHexColor(highlightedPaintData.Color);
            if (hexColor != PlayerDataProcessor.GetCarPaintHexColor(carID))
                PaintCar(originalColor);
        }

        materials.Clear();
        originalColor = Color.black;

        /* carObject = null;
        carID = null; */

        UICore.ClearContent(contentList);
        highlightedPaintData = null;
        selectedColorData = null;
    }

    private void Ini()
    {
        // GetMaterials();
        string closestPaintingID = GetClosestPaintID();

        // Color ce = Color.magenta;
        Color ce = new Color(0.4150943f, 0.2147884f, 0.0763617f);

        string hex = ColorUtility.ToHtmlStringRGB(ce);
        Debug.Log(hex);

        Color test;

        if (ColorUtility.TryParseHtmlString($"#{hex}", out test))
            Debug.Log(new Vector3(test.r, test.g, test.b) * 255f);
    }

    private void RecreateListOfColors()
    {
        List<PaintingColorData> paintingColorDatas = new List<PaintingColorData>(Resources.LoadAll<PaintingColorData>("Colors"));
        // Обдумать порядок сортировки, или вообще её присутствие
        // Внимание! Очень плохо написана, могут быть проблемы с оптимизацией/производительностью
        // Ещё она инвертированна
        paintingColorDatas.Sort(SortByColor);

        UICore.ClearContent(contentList);

        // string closestPaintingID = GetClosestPaintID();
        string paintedHexColor = PlayerDataProcessor.GetCarPaintHexColor(carID);

        foreach (var data in paintingColorDatas)
        {
            GameObject slotObject = Instantiate(slotPrefab, contentList);
            BaseItemSlot slot = slotObject.GetComponent<BaseItemSlot>();
            slot.Initialize(data, ClickedSlot);

            // string dataHexColor = ColorUtility.ToHtmlStringRGB(data.Color);
            string dataHexColor = GetHexColor(data.Color);

            // Debug.LogError(string.IsNullOrEmpty(paintedHexColor));

            if (highlightedPaintData == null)
            {
                if (!string.IsNullOrEmpty(paintedHexColor))
                {
                    if (dataHexColor == paintedHexColor)
                    {
                        highlightedPaintData = data;
                        ClickedSlot(slot, data);
                        SelectSlot(slot);
                    }
                }
                /* else
                    ClickedSlot(slot, data); */
            }

            // if (data.ID == closestPaintingID)
            /* if (dataHexColor == paintedHexColor)
            {
                // HightlightSlot(slot);
                ClickedSlot(slot, data);
                SelectSlot(slot);
            } */
        }
    }

    private void ClickedSlot(BaseItemSlot slot, object data)
    {
        if (slot == highlightedSlot)
            return;

        // HightlightSlot(slot);
        // Временно, пока не исправлю подсветку слота
        highlightedSlot = slot;

        PaintingColorData paintData = (PaintingColorData)data;

        UpdateUI(paintData);

        if (paintData == highlightedPaintData)
            return;

        highlightedPaintData = paintData;

        // UICore.PaintCar(carObject, paintData.Color);
        PaintCar(paintData.Color);
    }

    // Написано ооочень плохо
    // Инвертинованна
    private int SortByColor(PaintingColorData a, PaintingColorData b)
    {
        float hA, sA, vA;
        float hB, sB, vB;
        Color.RGBToHSV(a.Color, out hA, out sA, out vA);
        Color.RGBToHSV(b.Color, out hB, out sB, out vB);

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
        List<PaintingColorData> paintingColorDatas = new List<PaintingColorData>(Resources.LoadAll<PaintingColorData>("Colors"));

        string paintingID = null;
        float colorValue = 3f;

        foreach (var painting in paintingColorDatas)
        {
            float v = GetDifference(painting.Color, originalColor);

            if (v < colorValue)
            {
                paintingID = painting.ID;
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
        foreach (var material in materials)
            material.color = paintColor;
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

    private void ChangeBuySelectCarButton(PaintingColorData paintingColorData)
    {
        bool priceEnabled = true;
        string buttonText = "Purchase";

        string hexColor = ColorUtility.ToHtmlStringRGB(paintingColorData.Color);

        if (PlayerDataProcessor.GetCarPaintHexColor(carID) == hexColor)
        {
            priceEnabled = false;
            buttonText = "Painted";
        }

        if (priceEnabled)
            priceText.SetText($"Price {paintingColorData.Price}");

        priceText.enabled = priceEnabled;
        actionButton.GetComponentInChildren<TMP_Text>().SetText(buttonText);
    }

    private void UpdateUI(PaintingColorData paintData)
    {
        nameText.SetText(paintData.name);

        bool priceEnabled = true;
        string buttonText = "Purchase";

        // string hexColor = ColorUtility.ToHtmlStringRGB(paintData.Color);
        string hexColor = GetHexColor(paintData.Color);

        if (hexColor == PlayerDataProcessor.GetCarPaintHexColor(carID))
        {
            priceEnabled = false;
            buttonText = "Painted";
        }

        priceText.gameObject.SetActive(priceEnabled);

        if (priceEnabled)
            priceText.SetText($"Price {paintData.Price}");

        actionButton.GetComponentInChildren<TMP_Text>().SetText(buttonText);
    }

    public void BuyPaint()
    {
        if (!PlayerDataProcessor.BuyCarPaint(carID, highlightedPaintData))
            return;

        PlayerDataProcessor.Save();

        SelectSlot(highlightedSlot);
        UpdateUI(highlightedPaintData);
        // ChangeBuySelectCarButton(highlightedPaintData);

        // Не нужно, ведь машина уже перекрашивается при выделении слота
        // UICore.PaintCar(carObject, highlightedPaintData.Color);
    }
}
