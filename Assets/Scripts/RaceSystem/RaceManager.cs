using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance { get; private set; }

    // New
    public static RaceData selectedRaceData { get; private set; }
    private GameObject raceInstance;
    public static RaceStart activeRace { get; private set; }
    // ---


    public event Action OnRaceOver;

    public TimeSpan PrevRaceTime { get; private set; }
    public TimeSpan RaceTime { get; private set; }

    // Timer
    // private Coroutine timer;
    private float elapsedTime;
    private bool timerGoing;
    public event Action<TimeSpan> OnTimerUpdated;

    private void Awake()
    {
        /* if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); */

        instance = this;
        Debug.LogWarning(selectedRaceData);

        BeginRace();
    }

    public static void SelectRace(RaceData raceData) => selectedRaceData = raceData;

    public void BeginRace()
    {
        if (selectedRaceData == null)
            return;

        raceInstance = selectedRaceData.CreateRace();
        activeRace = raceInstance.GetComponentInChildren<RaceStart>();

        // FindObjectOfType<CarSpawn>().Initialize(PlayerDataProcessor.GetSelectedCarData().Prefab, null);
        // Переделать
        activeRace.OnRaceOver += RaceOver;
        activeRace.StartRace(selectedRaceData.ID);

        long prevRaceTime;
        int prevRaceScore;
        PlayerDataProcessor.ReadRaceSaveData(selectedRaceData.ID, out prevRaceTime, out prevRaceScore);

        PrevRaceTime = TimeSpan.FromTicks(prevRaceTime);
        StartTimer();
    }

#region Timer
    private void StartTimer()
    {
        /* if (timer != null)
            return; */

        elapsedTime = 0f;
        timerGoing = true;

        // timer = StartCoroutine(UpdateTimer());
    }

    private void Update() => UpdateTimer();

    private void UpdateTimer()
    {
        if (!timerGoing)
            return;

        elapsedTime += Time.deltaTime;
        RaceTime = TimeSpan.FromSeconds(elapsedTime);
        OnTimerUpdated?.Invoke(RaceTime);
    }

    // private void StopTimer() => StopCoroutine(timer);
    private void StopTimer()
    {
        // StopCoroutine(timer);

        timerGoing = false;
        elapsedTime += Time.deltaTime;
        RaceTime = TimeSpan.FromSeconds(elapsedTime);
        OnTimerUpdated?.Invoke(RaceTime);
    }

    /* private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            RaceTime = TimeSpan.FromSeconds(elapsedTime);
            OnTimerUpdated?.Invoke(RaceTime);

            yield return null;
        }
    } */
    #endregion

    private void RaceOver(string raceID)
    {
        StopTimer();

        float score = FindObjectOfType<ScoreCalculator>().GetTotalScore();

        // Debug
        TimeSpan targetTime;
        TimeSpan.TryParse(selectedRaceData.Goal, out targetTime);
        int targetScore;
        int.TryParse(selectedRaceData.Goal, out targetScore);

        Debug.LogWarning($"Target Time - {targetTime}");
        Debug.LogWarning($"Current Time - {RaceTime}");
        Debug.LogWarning($"Target Score - {targetScore}");
        Debug.LogWarning($"Current Score - {(int)score}");
        // ---

        // Добавить проверку лучшего времени
        /* switch (selectedRaceData.Goal)
        {
            default:
                DateTime dateTime = DateTime.Parse(selectedRaceData.GoalValue);

                // if (curTime <= dateTime.TimeOfDay.Ticks && (curTime < prevBestTime || prevBestTime == 0))
                //     PlayerDataProcessor.OverrideRaceBestTime(raceID, curTime);
                if (RaceTime <= dateTime.TimeOfDay && (RaceTime < PrevRaceTime || PrevRaceTime.Ticks == 0))
                    PlayerDataProcessor.OverrideRaceBestTime(raceID, RaceTime.Ticks);
                break;

            case RaceGoal.Score:
                int prevScore = PlayerDataProcessor.GetRaceScore(selectedRaceData.ID);
                if ((int)score >= int.Parse(selectedRaceData.GoalValue) && (int)score > prevScore)
                    PlayerDataProcessor.SetRaceScore(selectedRaceData.ID, (int)score);
                break;
        } */

        PlayerDataProcessor.WriteRaceCompletion(selectedRaceData, RaceTime, (int)score);

        // Deactivate car input and reset
        CarController carController = FindObjectOfType<CarController>();
        carController.InputEnabled = false;
        carController.GasInput = 0f;
        carController.BrakeInput = 1f;
        carController.SteerInput = 0f;
        carController.HandbrakeInput = false;
        carController.ClutchInput = 0f;

        OnRaceOver?.Invoke();
        
        // Del();
        SelectRace(null);
    }

    private void Del()
    {
        OnRaceOver = null;
        PrevRaceTime = new TimeSpan();
        RaceTime = new TimeSpan();
    }
}
