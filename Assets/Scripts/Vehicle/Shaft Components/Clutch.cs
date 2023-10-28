using UnityEngine;

public class Clutch : ShaftComponent, IInputReceiver
{
    [field: SerializeField] public ShaftComponent Output { get; internal set; }
    public float Value { get; private set; }

    InputHandler IInputReceiver.InputHandler { get => InputHandler; set => InputHandler = value; }
    public InputHandler InputHandler { get; private set; } = new();

    public override void Stream(in float inputVelocity, in float inputTorque, out float outputVelocity, out float outputTorque)
    {
        Value = InputHandler.GasInput;

        if (InputHandler.GasInput == 0f)
        {
            outputVelocity = inputVelocity;
            outputTorque = inputTorque;
            return;
        }

        float clutchTorque = inputTorque * InputHandler.GasInput;
        // test clutch torque
        Output.Stream(inputVelocity, clutchTorque, out outputVelocity, out outputTorque);
    }

    void IInputReceiver.OnInputHandlerChanger(in InputHandler inputHandler)
    {
    }
}
