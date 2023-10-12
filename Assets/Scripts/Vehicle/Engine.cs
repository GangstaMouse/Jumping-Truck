using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public AnimationCurve TorqueCurve;
    public float IdleRPM;
    public float MaxRPM;
    // public float LimitRPM;

    public float RPM { get; private set; }
    public float startFriction = 50f;
    public float frictionCoef = 0.02f;
    public float engineInertia = 0.2f;

    public float engineAngularVelocity;

    public const float RPMToRad = (Mathf.PI * 2f) / 60f;
    public const float RadToRPM = 1f / RPMToRad;
    private List<WheelCollider> driveWheels;
    private CarController car;

    private float physicsDeltaTime;
    private float driveWheelsSpeed;

    public float outputTorque { get; private set; }

    public float loadTorque;

    public void Initialize(CarController car, List<WheelCollider> driveWheels)
    {
        this.car = car;
        this.driveWheels = driveWheels;
    }

    private void OnEnable()
    {
        engineAngularVelocity = IdleRPM * RPMToRad;
        RPM = engineAngularVelocity * RadToRPM;
    }

    public void EngineUpdate(float deltaTime, float gas)
    {
        physicsDeltaTime = deltaTime;
        float gasValue = gas;


        // Acceleration
        float maxTorque = TorqueCurve.Evaluate(RPM);
        float friction = startFriction + (RPM * frictionCoef);
        float initialTorque = (maxTorque + friction) * gasValue;
        float effectiveTorque = initialTorque - friction;

        // float acceleration = effectiveTorque / engineInertia;
        float acceleration = (effectiveTorque - loadTorque) / engineInertia;
        float deltaAngularVelocity = acceleration * physicsDeltaTime;
        engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + deltaAngularVelocity, IdleRPM * RPMToRad, MaxRPM * RPMToRad);

        RPM = engineAngularVelocity * RadToRPM;

        outputTorque = engineAngularVelocity;
    }
    public void EngineUpdatet(float deltaTime, float gas)
    {
        if (TorqueCurve == null)
        {
            engineAngularVelocity = 0f;
            RPM = 0f;
            // Reset?
            return;
        }

        physicsDeltaTime = deltaTime;
        float gasValue = gas;

        float gearRatio = car.gearbox.totalGearRatio;

        // Test
        float totalAngularVelocity = 0f;
        float totalLinearVelocity = 0f;

        foreach (var wheel in driveWheels)
        {
            totalAngularVelocity += wheel.angularVelocity;
            totalLinearVelocity += wheel.angularVelocity * wheel.radius;
        }

        float middleAngularVelocity = totalAngularVelocity / driveWheels.Count;
        float clutchAngularVelocity = middleAngularVelocity * gearRatio;

        driveWheelsSpeed = totalLinearVelocity / driveWheels.Count;

        // engineAngularVelocity = 
        // Debug.Log(engineRPM);

        // engineAngularVelocity = Mathf.Clamp(((clutchAngularVelocity - engineAngularVelocity) * 0.1f * (TotalGearRatio != 0f ? 1f : 0f)) + engineAngularVelocity, IdleRPM * RPMToRad, MaxRPM * RPMToRad);
        // engineAngularVelocity = Mathf.Clamp(((clutchAngularVelocity - engineAngularVelocity) * 0.1f * (gearR != 0f ? 1f : 0f)) + engineAngularVelocity, IdleRPM * RPMToRad, MaxRPM * RPMToRad);
        engineAngularVelocity = Mathf.Clamp(((clutchAngularVelocity - engineAngularVelocity) * 1f * (gearRatio != 0f ? 1f : 0f)) + engineAngularVelocity, IdleRPM * RPMToRad, MaxRPM * RPMToRad);

        // Engine acceleration
        float maxtorque = TorqueCurve.Evaluate(RPM);
        float friction = startFriction + (RPM * frictionCoef);

        float curTorque = ((maxtorque + friction) * gasValue) - friction;

        // engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + ((curTorque / engineInertia) * deltaTime), IdleRPM * RPMToRad, MaxRPM * RPMToRad);
        engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + ((curTorque / engineInertia) * physicsDeltaTime), IdleRPM * RPMToRad, MaxRPM * RPMToRad);

        RPM = engineAngularVelocity * RadToRPM;

        // outputTorque = engineAngularVelocity;
        outputTorque = TorqueCurve.Evaluate(RPM);
    }
}
