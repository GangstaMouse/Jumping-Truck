using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text scoreText;
    
    private GameObject carObject;
    private ScoreCalculator calculator;

    void Awake()
    {
        foreach (var car in FindObjectsOfType<CarController>())
            if (car.controller == Controller.Player)
            {
                carObject = car.gameObject;
                break;
            }

        calculator = carObject.GetComponent<ScoreCalculator>();
    }

    void OnEnable()
    {
        RaceManager.instance.OnTimerUpdated += UpdateTimer;
        calculator.OnScoreChanges += SetScore;
    }

    void OnDisable()
    {
        RaceManager.instance.OnTimerUpdated -= UpdateTimer;
        calculator.OnScoreChanges -= SetScore;
    }

    private void UpdateTimer(TimeSpan time) => timeText.SetText($"Time - {time.ToString(@"mm\:ss\.fff")}");

    private void SetScore(float score) => scoreText.SetText($"Score: {Mathf.FloorToInt(score)}");
}
