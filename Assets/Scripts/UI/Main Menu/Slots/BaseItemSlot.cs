using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] protected Color m_DefaultColor = Color.white;
    [SerializeField] protected Color m_SelectedColor = Color.white;

    [SerializeField] protected Image highlightImage;
    [SerializeField] private Color highlightdefColor = Color.HSVToRGB(0f, 0f, 0.5f);
    [SerializeField] private Color highlightColor = Color.HSVToRGB(216f / 360f, 0.97f, 0.88f);

    public object Data { get; private set; }
    protected event Action<BaseItemSlot, object> OnClicked;

    private void Awake()
    {
        // m_DefaultColor
        // highlightdefColor
    }

    // Переименовать в Setup
    public virtual void Initialize(object data) => Data = data;
    public void Bind(Action<BaseItemSlot, object> action) => OnClicked = action;

    public void SetSelection(bool selection) => GetComponent<Image>().color = selection ? m_SelectedColor : m_DefaultColor;

    public void SetHovering(bool howering) => highlightImage.color = (howering ? highlightColor : highlightdefColor);

    public void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(this, Data);
#if UNITY_EDITOR

    private void OnValidate() => GetComponent<Image>().color = m_DefaultColor;
#endif
}
