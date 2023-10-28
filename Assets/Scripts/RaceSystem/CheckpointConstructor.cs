#if UNITY_EDITOR
using UnityEngine;

class CheckpointConstructor : MonoBehaviour
{
    [SerializeField] private float m_Width = 3f;

    [SerializeField] private Transform m_LeftMarker;
    [SerializeField] private Transform m_RightMarker;

    private void OnValidate()
    {
        if (m_LeftMarker != null)
        {
            Bounds leftMarkerBounds = m_LeftMarker.GetComponent<MeshFilter>().sharedMesh.bounds;
            m_LeftMarker.localPosition = new Vector3((-m_Width / 2f) - leftMarkerBounds.extents.x, m_LeftMarker.localPosition.y, m_LeftMarker.localPosition.z);
        }

        if (m_RightMarker != null)
        {
            Bounds rightMarkerBounds = m_LeftMarker.GetComponent<MeshFilter>().sharedMesh.bounds;
            m_RightMarker.localPosition = new Vector3((m_Width / 2f) + rightMarkerBounds.extents.x, m_RightMarker.localPosition.y, m_RightMarker.localPosition.z);
        }

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
            return;

        Bounds markerBounds = m_LeftMarker.GetComponent<MeshFilter>().sharedMesh.bounds;

        boxCollider.center = markerBounds.center;
        boxCollider.size = new Vector3(m_Width, markerBounds.size.y, markerBounds.size.z);
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
}
#endif
