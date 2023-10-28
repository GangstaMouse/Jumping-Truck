using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
class TireSoundEffect : MonoBehaviour
{
    private Vehicle m_Vehicle;
    private CustomWheelCollider m_WheelCollider;
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_Vehicle = GetComponentInParent<Vehicle>();
        m_WheelCollider = GetComponentInParent<CustomWheelCollider>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() => m_Vehicle.OnLateUpdateVisualEffects += UpdateSound;
    private void OnDisable() => m_Vehicle.OnLateUpdateVisualEffects -= UpdateSound;

    private void UpdateSound()
    {
        Vector2 slip = new float2(m_WheelCollider.CachedTireFrictionForce.x * 0.7f, m_WheelCollider.CachedTireFrictionForce.y) *
            m_WheelCollider.TireStiffnes;
        float volume = slip.magnitude * (m_WheelCollider.HasContact ? 2f : 0f);
        float pitch = FunctionsLibrary.MapRangeClamped(volume, 0f, 20f, 0.5f, 2f);

        m_AudioSource.volume = volume;
        m_AudioSource.pitch = pitch;
    }
}
