using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSelectionUIMenu<T> : BaseUI where T : class
{
    [SerializeField] protected RectTransform SelectionsContentTransform;

    protected List<BaseItemSlot> InstancedSlotsList = new();

    protected BaseItemSlot SelectedSlot;
    protected BaseItemSlot HoveredSlot;
    [SerializeField] private GameObject m_SlotPrefab;
    protected T SelectedDataAsset;

    protected override void ExitFromSubMenu()
    {
        ClearUI();
        base.ExitFromSubMenu();
    }

    protected void ClearUI()
    {
        SelectedSlot = null;
        HoveredSlot = null;
        SelectedDataAsset = null;

        foreach (RectTransform slotTransform in SelectionsContentTransform)
            Destroy(slotTransform.gameObject);

        InstancedSlotsList.Clear();
    }

    protected void RecreateSelectionList(List<T> list, Func<T, bool> tryFindBy = null, bool selectFirst = false)
    {
        foreach (var assetData in list)
        {
            GameObject slotObjectInstance = Instantiate(m_SlotPrefab, SelectionsContentTransform);
            BaseItemSlot slotInstance = slotObjectInstance.GetComponent<BaseItemSlot>();
            slotInstance.Initialize(assetData);
            InstancedSlotsList.Add(slotInstance);

            if (tryFindBy != null && tryFindBy(assetData))
            {
                OnSlotSelected(slotInstance, assetData);
                /* SelectSlot(slotInstance);
                HighlightSlot(slotInstance); */
            }
        }

        // Selecting first slot, after player vehicle was not found
        if (selectFirst && SelectedSlot == null && InstancedSlotsList.Count != 0)
        {
            BaseItemSlot firstSlot = InstancedSlotsList[0];
            OnSlotSelected(firstSlot, firstSlot.Data);
            /* SelectSlot(firstSlot);
            HighlightSlot(firstSlot); */
        }
    }

    protected void BindSlotsAction(Action<BaseItemSlot, object> bindAction)
    {
        foreach (var slotInstance in InstancedSlotsList)
            slotInstance.Bind(bindAction);
    }

    protected void OnSlotSelected(BaseItemSlot newSlot, object selectionAbstractData)
    {
        if (newSlot == SelectedSlot)
            return;

        SelectSlot(newSlot);

        SelectedDataAsset = selectionAbstractData as T;
        SelectionLogic(SelectedDataAsset);
    }

    protected abstract void SelectionLogic(T selectedData);

    protected abstract void UpdateSelectionUI(T selectedDataAsset);

    protected void SelectSlot(BaseItemSlot newSlot)
    {
        if (SelectedSlot != null)
            SelectedSlot.SetSelection(false);

        SelectedSlot = newSlot;
        SelectedSlot.SetSelection(true);
    }

    protected void HighlightSlot(BaseItemSlot newSlot)
    {
        if (HoveredSlot != null)
            HoveredSlot.SetHovering(false);

        HoveredSlot = newSlot;
        HoveredSlot.SetHovering(true);
    }
}

public abstract class BasePurchaseUIMenu<T> : BaseSelectionUIMenu<T> where T : OwnableItemDataSO
{
    [SerializeField] protected Button ActionButton;
    protected TMP_Text ActionText;

    protected override void Awake()
    {
        base.Awake();
        ActionButton.onClick.AddListener(() => OnSelectPurchaseButton(SelectedDataAsset));
        ActionText = ActionButton.gameObject.GetComponentInChildren<TMP_Text>();
    }
    protected abstract void OnSelectPurchaseButton(T dataAsset);
}
