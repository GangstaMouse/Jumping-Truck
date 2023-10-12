using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaseItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] protected Color defaultColor = Color.white;
    [SerializeField] protected Color selectedColor = Color.white;

    [SerializeField] protected Image highlightImage;
    [SerializeField] private Color highlightdefColor = Color.HSVToRGB(0f, 0f, 0.5f);
    [SerializeField] private Color highlightColor = Color.HSVToRGB(216f / 360f, 0.97f, 0.88f);

    public object data;
    protected event Action<BaseItemSlot, object> onClick;

    // Переименовать в Setup
    public virtual void Initialize(object data, Action<BaseItemSlot, object> action)
    {
        this.data = data;
        onClick = action;
    }

    public void Select() => GetComponent<Image>().color = selectedColor;

    public void Deselect() => GetComponent<Image>().color = defaultColor;

    public void Highlight(bool newHighlight) => highlightImage.color = (newHighlight ? highlightColor : highlightdefColor);

    public void OnPointerClick(PointerEventData eventData) => onClick?.Invoke(this, data);

#if UNITY_EDITOR
    private void OnValidate() => GetComponent<Image>().color = defaultColor;
#endif
}
