using UnityEngine;

public class Differential : ShaftComponent
{
    [field: SerializeField] public float GearRatio { get; internal set; } = 0.8f;
    [field: SerializeField] public ShaftComponent LeftOutput { get; internal set; }
    [field: SerializeField] public ShaftComponent RightOutput { get; internal set; }

    // welded differential type
    public override void Stream(in float inputVelocity, in float inputTorque, out float outputVelocity, out float outputTorque)
    {
        float torque = (inputTorque * GearRatio) / 2.0f;
        float velocity = inputVelocity / GearRatio;
        LeftOutput.Stream(velocity, torque, out float leftOutputVelocity, out float leftOutputTorque);
        RightOutput.Stream(velocity, torque, out float rightOutputVelocity, out float rightOutputTorque);
        
        outputVelocity = (leftOutputVelocity + rightOutputVelocity) / 2.0f * GearRatio;
        outputTorque = (leftOutputTorque + rightOutputTorque) / 2.0f / GearRatio;
    }
}
