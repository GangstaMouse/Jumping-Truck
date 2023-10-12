using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Differential : MonoBehaviour
{
    public float Ratio = 1f;
    public List<WheelCollider> wheels = new List<WheelCollider>();
    [SerializeField] private DifferentialType type;

    public enum DifferentialType { Open, Locked }

    public float GetOutputTorque(float inputTorque) => (inputTorque * Ratio) / wheels.Count;

    // public float GetInputShaftVelocity(float outputShaftVelocity)
    public float GetInputShaftVelocity()
    {
        float totalAngularVelocity = 0f;
        foreach (var wheel in wheels)
            totalAngularVelocity += wheel.angularVelocity;

        float meanAngularVelocity = totalAngularVelocity / wheels.Count;
        return meanAngularVelocity * Ratio;
    }

    public void TransferOutputTorqueToWheels(float deltaTime, float inputTorque)
    {
        if (type == DifferentialType.Locked)
        {
            GetLockedTorque(deltaTime, inputTorque);
            return;
        }

        float outputTorque = GetOutputTorque(inputTorque);

        foreach (var wheel in wheels)
            // Временная мера
            // wheel.driveTorque = outputTorque;
            wheel.inputTorque = outputTorque;
    }

    private void GetLockedTorque(float deltaTime, float inputTorque)
    {
        float inertia = (wheels[0].inertia + wheels[1].inertia) / 2f;
        float lockTorque = ((wheels[0].angularVelocity - wheels[1].angularVelocity) / 2f) * inertia;
        float t = GetOutputTorque(inputTorque);

        wheels[0].inputTorque = t - lockTorque;
        wheels[1].inputTorque = t + lockTorque;
    }
}
