using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneSelectionUIMenu : BaseSelectionUIMenu<SceneData>
{
    [Header("References")]
    [SerializeField] private GameObject backButton;

    [SerializeField] private TMP_Text selectedSceneText;
    [SerializeField] private TMP_Text selectionText;
    [SerializeField] private RaceSelectionUIMenu m_RaceSelectionUIMenu;
    private RaceData m_SelectedRace;

    protected override void Awake()
    {
        base.Awake();
        m_RaceSelectionUIMenu.OnRaceSelected += OnRaceSelected;
    }

    protected override void DrawUI()
    {
        RefreshHeaderText();
        List<SceneData> sceneDatasAssets = new(ItemDataSO.LoadAssetsFromPath<SceneData>("Scenes"));
        RecreateSelectionList(sceneDatasAssets);
        BindSlotsAction(OnSlotSelected);
    }

    private void OnRaceSelected(RaceData dataAsset)
    {
        m_SelectedRace = dataAsset;
        RaceManager.SelectRace(m_SelectedRace);
        SceneLoader.LoadScene(SelectedDataAsset.SceneName);
    }

    public void StartRace()
    {
        if (SelectedDataAsset == null || m_SelectedRace == null || string.IsNullOrEmpty(PlayerSavesManager.Vehicle.SelectedCarIdentifier))
            return;

        // SceneLoader.instance.LoadScene(SelectedDataAsset.SceneName);
        // SceneLoader.instance.LoadScene(SelectedDataAsset.SceneName, () => RaceManager.instance.StartRace(selectedRace));
        RaceManager.SelectRace(m_SelectedRace);
        SceneLoader.LoadScene(SelectedDataAsset.SceneName);
    }

    private void RefreshHeaderText()
    {
        string selectionString;

        if (SelectedDataAsset == null)
        {
            selectedSceneText.gameObject.SetActive(false);
            selectionString = "Select Track";
        }
        else
        {
            selectedSceneText.gameObject.SetActive(true);
            selectedSceneText.SetText($"{SelectedDataAsset.SceneName} |");
            selectionString = "Select Race";
        }

        selectionText.SetText(selectionString);
    }

    public void ShowBackButton(bool newShow) => backButton.SetActive(newShow);

    public void BackButton()
    {
/*         if (SelectedDataAsset != null)
        {
            SelectedDataAsset = null;
            m_SelectedRace = null;
            RecreateListOfScenes();
        }
        else
        {
            SetVisibility(false);
        } */
    }

    protected override void SelectionLogic(SceneData selectedData)
    {
        SelectedDataAsset = selectedData;
        m_SelectedRace = null;
        m_RaceSelectionUIMenu.Initialize(SelectedDataAsset);
        m_RaceSelectionUIMenu.Enter();

        // RecreateListOfRaces();
    }

    protected override void UpdateSelectionUI(SceneData selectedDataAsset)
    {
        throw new System.NotImplementedException();
    }
}
