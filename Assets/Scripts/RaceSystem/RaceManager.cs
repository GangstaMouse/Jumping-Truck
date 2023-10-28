using System;
using System.Threading.Tasks;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    public static RaceData SelectedRaceData { get; private set; }
    public static RaceStart ActiveRace { get; private set; }
    private GameObject m_RaceInstanceObject;

    public event Action OnRaceOver;

    public TimeSpan PrevRaceTime { get; private set; }
    public TimeSpan RaceTime { get; private set; }

    // Timer
    private float m_ElapsedTime;
    private bool m_IsTimerRunning;
    public event Action<TimeSpan> OnTimerUpdated;

    private void Awake()
    {
        Instance = this;

        if (SelectedRaceData != null)
        {
            m_RaceInstanceObject = SelectedRaceData.CreateRace();
            ActiveRace = m_RaceInstanceObject.GetComponent<RaceStart>();

            ActiveRace.OnRaceOver += RaceOver;
            ActiveRace.StartRace(SelectedRaceData.Identifier);

            long prevRaceTime;
            int prevRaceScore;
            PlayerSavesManager.Race.ReadRaceSaveData(SelectedRaceData.Identifier, out prevRaceTime, out prevRaceScore);

            PrevRaceTime = TimeSpan.FromTicks(prevRaceTime);
        }
    }

    private void Start()
    {
        if (SelectedRaceData != null)
            BeginRace();
    }

    public static void SelectRace(RaceData raceData) => SelectedRaceData = raceData;

    public void BeginRace()
    {
        Vehicle vehicle = FindObjectOfType<Vehicle>();
        vehicle.gameObject.AddComponent<PlayerInputHandlerInstancer>();
        vehicle.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);

        StartTimer();
    }

    #region Timer
    // Need to test it
    private async void StartTimer()
    {
        m_ElapsedTime = 0.0f;
        m_IsTimerRunning = true;

        while (m_IsTimerRunning)
        {
            m_ElapsedTime += Time.deltaTime;
            RaceTime = TimeSpan.FromSeconds(m_ElapsedTime);
            OnTimerUpdated?.Invoke(RaceTime);

            await Task.Yield();
        }
    }

    private void StopTimer() => m_IsTimerRunning = false;
    #endregion

    private void RaceOver(string raceID)
    {
        StopTimer();

        float score = FindObjectOfType<ScoreCalculator>().GetTotalScore();

#if UNITY_EDITOR
        TimeSpan targetTime;
        TimeSpan.TryParse(SelectedRaceData.Goal, out targetTime);
        int targetScore;
        int.TryParse(SelectedRaceData.Goal, out targetScore);

        Debug.LogWarning($"Target Time - {targetTime}");
        Debug.LogWarning($"Current Time - {RaceTime}");
        Debug.LogWarning($"Target Score - {targetScore}");
        Debug.LogWarning($"Current Score - {(int)score}");
#endif

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

        PlayerSavesManager.Race.WriteRaceCompletion(SelectedRaceData, RaceTime, (int)score);

        // Deactivate car input and reset
        Vehicle vehicle = FindObjectOfType<Vehicle>();
        var newInst = vehicle.gameObject.AddComponent<StaticInputHandlerInstancer>();
        newInst.GasInput = 0;
        newInst.SteerInput = 0;
        newInst.BrakeInput = 1;
        newInst.HandbrakeInput = 1;

        OnRaceOver?.Invoke();
        SelectRace(null);
    }
}
