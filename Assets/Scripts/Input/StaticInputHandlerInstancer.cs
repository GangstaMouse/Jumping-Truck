public class StaticInputHandlerInstancer : InputHandlerInstancer<StaticInputHandler>
{
    public float GasInput;
    public float SteerInput;
    public float BrakeInput;
    public float HandbrakeInput;

    protected override void Initialize()
    {
        InputHandlerInstance = new StaticInputHandler(GasInput, SteerInput, BrakeInput, HandbrakeInput);
    }
}

public class StaticInputHandler : InputHandler
{
    public StaticInputHandler(float gasInput, float steerInput, float brakeInput, float handbrakeInput)
    {
        GasInput = gasInput;
        SteerInput = steerInput;
        BrakeInput = brakeInput;
        HandbrakeInput = handbrakeInput;
    }
}
