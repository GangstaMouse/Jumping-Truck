using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceSelectionUIMenu : BaseSelectionUIMenu<RaceData>
{
    [SerializeField] private TMP_Text m_HeaderText;
    public event Action<RaceData> OnRaceSelected;
    private SceneData m_SceneData;

    public void Initialize(SceneData sceneData)
    {
        m_SceneData = sceneData;
    }

    protected override void DrawUI()
    {
        ClearUI();
        m_HeaderText.SetText("Select race");
        List<RaceData> datasAssets = new(ItemDataSO.LoadAssetsFromPath<RaceData>($"Races {m_SceneData.SceneName}"));
        RecreateSelectionList(datasAssets);
        BindSlotsAction(OnSlotSelected);
    }

    protected override void SelectionLogic(RaceData selectedData)
    {
        OnRaceSelected?.Invoke(selectedData);
    }

    protected override void UpdateSelectionUI(RaceData selectedDataAsset)
    {
    }
}
