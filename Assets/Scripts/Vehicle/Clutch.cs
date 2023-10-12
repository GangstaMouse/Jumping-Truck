using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clutch : MonoBehaviour
{
    public const float RPMToRad = (Mathf.PI * 2f) / 60f;
    public const float RadToRPM = 1f / RPMToRad;

    public float EngineMaxTorque;
    public float ClutchCapacity = 1.3f;
    private float ClutchMaxTorque;
    public float ClutchStiffnes = 40f;
    public float ClutchDamping = 0.7f;

    public float torque { get; private set; }

    private void Awake()
    {
        ClutchMaxTorque = EngineMaxTorque * ClutchCapacity;
    }

    // Warning!!! Сейчас движок захлёбывается своими же мощностями, т.е. сцепление нагружает движок его же агловой скоростью когда нету скорости с колёс.
    // Поэтому есть идея отнимать от наверное clutchSlip ещё раз скорость движка, что бы получился 0 при отсутствии скорости колёс
    public void UpdatePhysics(float outputShaftVelocity, float engineAngularVelocity, float gearRatio, float clutchValue)
    {
        float clutchVelocity = outputShaftVelocity;
        float clutchSlip = (engineAngularVelocity - clutchVelocity) * Mathf.Sign(Mathf.Abs(gearRatio));
        float clutchLock = Mathf.Min((gearRatio == 0f ? 1f : 0f) + clutchValue, 1f);
        float t = Mathf.Clamp(clutchSlip * clutchLock * ClutchStiffnes, -ClutchMaxTorque, ClutchMaxTorque);
        torque = t + ((torque - t) * ClutchDamping);
    }

    public void SetTorque(float outputShaftVelocity, float engineAngularVelocity)
    {
        torque = (outputShaftVelocity + engineAngularVelocity) / 2f;
    }
}
