using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TireSoundEffect : MonoBehaviour
{
    private AudioSource audioSource;
    private WheelCollider wheelCollider;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        wheelCollider = GetComponentInParent<WheelCollider>();
    }

    private void OnEnable() => wheelCollider.OnPostPhysics += UpdateSound;

    private void OnDisable() => wheelCollider.OnPostPhysics -= UpdateSound;

    private void UpdateSound()
    {
        Vector2 slip = new Vector2(wheelCollider.TireSlip.x * 0.7f, wheelCollider.TireSlip.y);
        float volume = slip.magnitude * (wheelCollider.grounded ? 1f : 0f);
        float pitch = FunctionsLibrary.MapRangeClamped(volume, 0f, 20f, 0.5f, 2f);

        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }
}
