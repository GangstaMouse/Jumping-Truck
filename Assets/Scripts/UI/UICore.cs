using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UICore
{
    public static void HightlightSlot(BaseItemSlot newSlot, ref BaseItemSlot refSlot)
    {
        if (refSlot != null)
            refSlot.Deselect();

        refSlot = newSlot;
        refSlot.Select();
    }

    public static void SelectSlot(BaseItemSlot newSlot, ref BaseItemSlot refSlot)
    {
        if (refSlot != null)
            refSlot.Highlight(false);

        refSlot = newSlot;
        refSlot.Highlight(true);
    }

    public static void LoadPlayerCarTuning(GameObject carObject, string carID)
    {
        // Load Upgrades
        List<CarPartSocket> partSockets = new List<CarPartSocket>(carObject.GetComponentsInChildren<CarPartSocket>());

        foreach (var socket in partSockets)
        {
            string partID = PlayerDataProcessor.GetInstalledPartID(carID, socket.ID);
            CarPartData partData = CarPartData.GetAssetByID(partID);

            if (partData != null)
                socket.PartData = partData;
        }

        // Load Paint Color
        string hexColor = PlayerDataProcessor.GetCarPaintHexColor(carID);
        Color paintColor;

        if (ColorUtility.TryParseHtmlString(hexColor, out paintColor))
        // if (ColorUtility.TryParseHtmlString($"#{hexColor}", out paintColor))
            PaintCar(carObject, paintColor);
        else
            Debug.LogWarning($"HexColor <<{hexColor}>> is invalid");
    }

    public static void PaintCar(GameObject carObject, Color paintColor)
    {
        List<Material> materials = new List<Material>(FunctionsLibrary.GetListOfMaterialsByName(carObject, "Car Paint"));

        foreach (var material in materials)
            material.color = paintColor;
    }

    public static void ClearContent(Transform content)
    {
        foreach (RectTransform child in content)
            MonoBehaviour.Destroy(child.gameObject);
    }
}
