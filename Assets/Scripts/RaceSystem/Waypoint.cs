using System;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // [Header("Settings")]
    [field: SerializeField] public bool Active { get; private set; } = true;
    [field: SerializeField] public WaypointType Type { get; private set; } = WaypointType.Checkpoint;

    // [Header("References")]
    [field: SerializeField] public Waypoint NextWaypoint { get; private set; }
    public event Action<WaypointType> OnPassed;

    private void OnTriggerEnter(Collider collider)
    {
        // Test it
        if (collider.GetComponentInParent<Vehicle>() != null)
            Reached();
    }

    private void Awake() => SetActive(Active);

    public void SetActive(bool active)
    {
        Active = active;
        enabled = Active;
    }

    public void Reached()
    {
        SetActive(false);
        OnPassed?.Invoke(Type);
        if (NextWaypoint)
            NextWaypoint.SetActive(true);
        OnPassed = null;
    }
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.3f);

        if (NextWaypoint != null)
            GizmosLibrary.DrawArrowedLine(transform.position, NextWaypoint.transform.position);
    }
#endif
}
