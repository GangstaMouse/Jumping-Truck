using System;
using UnityEngine;
using TMPro;

public class RaceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text m_TimeText;
    [SerializeField] private TMP_Text m_ScoreText;

    void Awake()
    {
    }

    void OnEnable()
    {
        RaceManager.Instance.OnTimerUpdated += UpdateTimer;
        ScoreCalculator.OnScoreChanged += UpdateScore;
    }

    void OnDisable()
    {
        RaceManager.Instance.OnTimerUpdated -= UpdateTimer;
        ScoreCalculator.OnScoreChanged -= UpdateScore;
    }

    private void UpdateTimer(TimeSpan time) => m_TimeText.SetText($"Time - {time.ToString(@"mm\:ss\.fff")}");
    private void UpdateScore(float score) => m_ScoreText.SetText($"Score: {Mathf.FloorToInt(score)}");
}
