using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointConstructor : MonoBehaviour
{
    [SerializeField] private float width = 3f;

    [SerializeField] private Transform leftMarker;
    [SerializeField] private Transform rightMarker;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (leftMarker != null)
        {
            Bounds leftMarkerBounds = leftMarker.GetComponent<MeshFilter>().sharedMesh.bounds;
            leftMarker.localPosition = new Vector3((-width / 2f) - leftMarkerBounds.extents.x, leftMarker.localPosition.y, leftMarker.localPosition.z);
        }

        if (rightMarker != null)
        {
            Bounds rightMarkerBounds = leftMarker.GetComponent<MeshFilter>().sharedMesh.bounds;
            rightMarker.localPosition = new Vector3((width / 2f) + rightMarkerBounds.extents.x, rightMarker.localPosition.y, rightMarker.localPosition.z);
        }

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
            return;

        Bounds markerBounds = leftMarker.GetComponent<MeshFilter>().sharedMesh.bounds;

        boxCollider.center = markerBounds.center;
        boxCollider.size = new Vector3(width, markerBounds.size.y, markerBounds.size.z);
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
    }
    #endif
}
