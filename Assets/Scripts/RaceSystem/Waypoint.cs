using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool active;
    [SerializeField] private bool reached;

    [Header("References")]
    [SerializeField] private List<Waypoint> nextWaypoints = new List<Waypoint>();

    [SerializeField] bool isFinish;

    private Waypoint prevWaypoint;

    private void OnTriggerEnter(Collider collider)
    {
        if (reached)
            return;

        CarController car = collider.GetComponentInParent<CarController>();

        if (car != null)
            SetReach(true);
    }

    public void SetActive(bool newActive) => active = newActive;

    public void SetPrevWaypoint(Waypoint waypoint) => prevWaypoint = waypoint;

    public void SetReach(bool newReach)
    {
        reached = newReach;

        if (!reached)
            return;

        SetActive(false);
        if (DoesIsBranch(prevWaypoint))
            DisableParallelWaypoints();
            // Debug.Log(this);

        // Временно
        if (isFinish)
        {
            GetComponentInParent<RaceStart>().OverRace();
        }

        foreach (var waypoint in nextWaypoints)
        {
            waypoint.SetPrevWaypoint(this);
            waypoint.SetActive(true);
        }
    }

    private bool DoesIsBranch(Waypoint waypoint)
    {
        if (prevWaypoint != null && waypoint.nextWaypoints.Count > 1)
            return true;

        return false;
    }

    private void DisableParallelWaypoints()
    {
        List<Waypoint> parallelWaypoints = new List<Waypoint>(prevWaypoint.nextWaypoints);
        parallelWaypoints.Remove(this);

        foreach (var waypoint in parallelWaypoints)
            waypoint.SetActive(false);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.3f);

        foreach (var waypoint in nextWaypoints)
            if (waypoint != null)
                GizmosLibrary.DrawArrowedLine(transform.position, waypoint.transform.position);
    }
    #endif
}

public enum WaypointType { Start, Checkpoint, Finish }
