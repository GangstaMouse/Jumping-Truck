using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStart : MonoBehaviour
{
    [SerializeField] private List<Waypoint> waypoints = new List<Waypoint>();

    private string raceID;
    public event Action<string> OnRaceOver;

    public void StartRace(string raceID)
    {
        this.raceID = raceID;

        foreach (var waypoint in waypoints)
            waypoint.SetActive(true);
    }

    public void OverRace() => OnRaceOver?.Invoke(raceID);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var waypoint in waypoints)
            if (waypoint != null)
                // GizmosLibrary.DrawArrowedLine(transform.position, waypoint.transform.position);
                GizmosLibrary.DrawTwoColoredArrowedLine(transform.position, waypoint.transform.position, Color.green, Color.white);
    }
    #endif
}
