using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceStart : MonoBehaviour
{
    [field: SerializeField] public Waypoint StartWaypoint { get; private set; }
    public event Action<string> OnRaceOver;
    private string m_RaceID;

    public void StartRace(string raceID)
    {
        m_RaceID = raceID;

        List<Waypoint> waypoints = new(GetComponentsInChildren<Waypoint>());

        foreach (var waypoint in waypoints)
            waypoint.OnPassed += OnWaypointPassed;

        StartWaypoint.SetActive(true);
    }

    public void OnWaypointPassed(WaypointType waypointType)
    {
        if (waypointType == WaypointType.Finish)
            OnRaceOver?.Invoke(m_RaceID);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        /* int iterations = 0;
        foreach (var waypoint in StartWaypoint)
            if (waypoint != null)
                // GizmosLibrary.DrawArrowedLine(transform.position, waypoint.transform.position);
                GizmosLibrary.DrawTwoColoredArrowedLine(transform.position, waypoint.transform.position, Color.green, Color.white); */
    }
#endif
}
