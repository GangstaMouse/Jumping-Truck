using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private TMP_Text previousTimeText;
    [SerializeField] private TMP_Text currentTimeText;

    void OnEnable() => RaceManager.Instance.OnRaceOver += Show;

    void OnDisable() => RaceManager.Instance.OnRaceOver -= Show;

    private void Show()
    {
        rectTransform.gameObject.SetActive(true);
        UpdateTime();
    }

    private void UpdateTime()
    {
        TimeSpan prevTime = RaceManager.Instance.PrevRaceTime;
        TimeSpan curTime = RaceManager.Instance.RaceTime;

        previousTimeText.SetText($"Previous time - {prevTime.ToString(@"mm\:ss\.fff")}");
        currentTimeText.SetText($"Current time - {curTime.ToString(@"mm\:ss\.fff")}");
    }

    public void ReturnToMainMenu() => SceneLoader.LoadScene("MainMenu");
}
