using UnityEngine;
using TMPro;

class CashValue : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;

    private void OnEnable()
    {
        PlayerSavesManager.OnCashChanged += SetCashText;
        SetCashText(PlayerSavesManager.Cash);
    }

    private void OnDisable() => PlayerSavesManager.OnCashChanged -= SetCashText;

    private void SetCashText(int value) => m_Text.SetText($"Cash: {value}");
}
