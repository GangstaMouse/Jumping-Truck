using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Race Data")]
public sealed class RaceData : ItemDataSO
{
    [SerializeField] private GameObject m_RacePrefab;
    public string Text = "None";
    public bool Unlocked;

    [Multiline] [SerializeField] private string m_Goal = "time_00:01:00";
    public string Goal => m_Goal;
    [field: SerializeField] public int CashReward { get; private set; } = 500;
    [field: SerializeField] public int ExperienceReward { get; private set; } = 100;

    public override string ResourceFolderName => "Races";

    public GameObject CreateRace() => Instantiate(m_RacePrefab);
}
