using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CarSlot : BaseItemSlot, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    /* [SerializeField] private Image highlightImage;

    [SerializeField] private Color highlightdefColor = Color.HSVToRGB(0f, 0f, 0.5f);
    [SerializeField] private Color highlightColor = Color.HSVToRGB(216f / 360f, 0.97f, 0.88f); */

    /* private new CarData data;
    private new event Action<BaseItemSlot, CarData> onClick; */

    public override void Initialize(object data)
    {
        base.Initialize(data);

        CarDataSO carData = (CarDataSO)this.Data;

        nameText.SetText(carData.Name);
        priceText.SetText($"Price: {carData.Price}");
    }

    // public void Highlight(bool newHighlight) => highlightImage.color = (newHighlight ? highlightColor : highlightdefColor);

    // public new void OnPointerClick(PointerEventData eventData) => onClick?.Invoke(this, data);
}
