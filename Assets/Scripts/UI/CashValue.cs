using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CashValue : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        PlayerDataProcessor.OnCashValueChanged += SetCashText;
        SetCashText(PlayerDataProcessor.GetCash());
    }

    private void OnDisable() => PlayerDataProcessor.OnCashValueChanged -= SetCashText;

    private void SetCashText(int value) => text.SetText($"Cash: {value}");
}
