using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class WheelVisual : MonoBehaviour
{
    [System.NonSerialized] private CustomWheelCollider m_WheelCollider;

    private void Awake() => m_WheelCollider = GetComponentInParent<CustomWheelCollider>();
    private void FixedUpdate() => UpdateWheelPosition();
    private void UpdateWheelPosition()
    {
        transform.localPosition = -math.up() * m_WheelCollider.CurrentSuspensionLenght;
        transform.rotation = math.mul(math.normalize(transform.rotation),
                    quaternion.AxisAngle(math.right(), math.radians(m_WheelCollider.WheelAngularVelocity)));
    }
#if UNITY_EDITOR
    private void OnValidate() => m_WheelCollider = GetComponentInParent<CustomWheelCollider>();
    private void OnDrawGizmos() => UpdateWheelPosition();
#endif
}
