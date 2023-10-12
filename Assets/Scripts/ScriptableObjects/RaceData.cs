using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Race Data")]
public class RaceData : BaseItemData
{
    public GameObject RacePrefab;
    public string Text = "None";
    public bool Unlocked;

    [Multiline]
    public string Goal = "time_00:01:00";
    public int CashReward = 500;
    public int ExperienceReward = 100;

    public GameObject CreateRace() => Instantiate(RacePrefab);
}
