using UnityEngine;

public abstract class ShaftComponent : MonoBehaviour
{
    public abstract void Stream(in float inputVelocity, in float inputTorque, out float outputVelocity, out float outputTorque);
}
