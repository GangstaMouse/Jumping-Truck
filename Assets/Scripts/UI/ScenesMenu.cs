using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScenesMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject backButton;

    [SerializeField] private TMP_Text selectedSceneText;
    [SerializeField] private TMP_Text selectionText;

    [SerializeField] private RectTransform scenesContentList;
    [SerializeField] private RectTransform racesContentList;

    [Header("Prefab")]
    [SerializeField] private GameObject sceneSlotPrefab;
    [SerializeField] private GameObject raceSlotPrefab;

    private bool menuVisibility;

    private BaseItemSlot highlightedSceneSlot;
    private SceneData selectedScene;

    private BaseItemSlot highlightedRaceSlot;
    private RaceData selectedRace;

    public void ToggleVisibility() => SetVisibility(!menuVisibility);

    public void SetVisibility(bool newVisibility)
    {
        menuVisibility = newVisibility;
        gameObject.SetActive(menuVisibility);

        if (menuVisibility)
            RecreateListOfScenes();
        else
        {
            highlightedSceneSlot = null;
            selectedScene = null;

            highlightedRaceSlot = null;
            selectedRace = null;

            UICore.ClearContent(scenesContentList);
            UICore.ClearContent(racesContentList);
        }
    }

    private void RecreateListOfScenes()
    {
        UICore.ClearContent(scenesContentList);

        RefreshHeaderText();

        List<SceneData> sceneDatas = GetScenes();

        foreach (var data in sceneDatas)
        {
            GameObject newSlot = Instantiate(sceneSlotPrefab, scenesContentList);
            BaseItemSlot sceneSlot = newSlot.GetComponent<BaseItemSlot>();
            sceneSlot.Initialize(data, HighlightSceneSlot);

            /* if (highlightedSlot == null)
                HighlightSlot(sceneSlot, data); */
        }
    }

    // private void HighlightSceneSlot(BaseItemSlot slot)
    private void HighlightSceneSlot(BaseItemSlot slot, object data)
    {
        if (highlightedSceneSlot != null)
            highlightedSceneSlot.Deselect();

        highlightedSceneSlot = slot;
        highlightedSceneSlot.Select();

        SceneData sceneData = (SceneData)highlightedSceneSlot.data;

        if (selectedScene != sceneData && sceneData.Unlocked)
            SelectScene(sceneData);
    }

    // private void HighlightRaceSlot(BaseItemSlot slot)
    private void HighlightRaceSlot(BaseItemSlot slot, object data)
    {
        if (highlightedRaceSlot != null)
            highlightedRaceSlot.Deselect();

        highlightedRaceSlot = slot;
        highlightedRaceSlot.Select();

        RaceData raceData = (RaceData)highlightedRaceSlot.data;

        if (selectedScene != raceData && raceData.Unlocked)
            SelectRace(raceData);
    }

    private void SelectScene(SceneData sceneData)
    {
        selectedScene = sceneData;
        selectedRace = null;

        RecreateListOfRaces();
    }

    private void RecreateListOfRaces()
    {
        UICore.ClearContent(racesContentList);

        RefreshHeaderText();

        List<RaceData> raceDatas = GetRacesFromScene(selectedScene);

        foreach (var data in raceDatas)
        {
            GameObject newSlot = Instantiate(raceSlotPrefab, racesContentList);
            BaseItemSlot sceneSlot = newSlot.GetComponent<BaseItemSlot>();
            sceneSlot.Initialize(data, HighlightRaceSlot);

            /* if (highlightedSlot == null)
                HighlightSlot(sceneSlot, data); */
        }
    }

    private void SelectRace(RaceData raceData) => selectedRace = raceData;

    private List<SceneData> GetScenes() => new List<SceneData>(Resources.LoadAll<SceneData>("Scenes"));

    private List<RaceData> GetRacesFromScene(SceneData sceneData) => new List<RaceData>(Resources.LoadAll<RaceData>($"Races/{sceneData.SceneName}"));

    public void StartRace()
    {
        if (selectedScene == null || selectedRace == null || PlayerDataProcessor.DoesCarSelected())
            return;

        // SceneLoader.instance.LoadScene(selectedScene.SceneName);
        // SceneLoader.instance.LoadScene(selectedScene.SceneName, () => RaceManager.instance.StartRace(selectedRace));
        RaceManager.SelectRace(selectedRace);
        SceneLoader.instance.LoadScene(selectedScene.SceneName);
    }

    private void RefreshHeaderText()
    {
        string selectionString;

        if (selectedScene == null)
        {
            selectedSceneText.gameObject.SetActive(false);
            selectionString = "Select Track";
        }
        else
        {
            selectedSceneText.gameObject.SetActive(true);
            selectedSceneText.SetText($"{selectedScene.SceneName} |");
            selectionString = "Select Race";
        }

        selectionText.SetText(selectionString);
    }

    public void ShowBackButton(bool newShow) => backButton.SetActive(newShow);

    public void BackButton()
    {
        if (selectedScene != null)
        {
            selectedScene = null;
            selectedRace = null;
            RecreateListOfScenes();
        }
        else
        {
            SetVisibility(false);
        }
    }
}
