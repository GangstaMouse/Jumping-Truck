public class PlayerInputHandlerInstancer : InputHandlerInstancer<PlayerInputHandler>
{
    protected override void Initialize()
    {
        InputHandlerInstance = new();
    }
}

public class PlayerInputHandler : InputHandler
{
    public readonly PlayerControls PlayerControls;

    public override float GasRawInput => PlayerControls.Vehicle.Gas.ReadValue<float>();
    public override float GasInput => IsFlipped ? PlayerControls.Vehicle.Brake.ReadValue<float>() : PlayerControls.Vehicle.Gas.ReadValue<float>();
    public override float SteerInput => PlayerControls.Vehicle.Steer.ReadValue<float>();
    public override float BrakeRawInput => PlayerControls.Vehicle.Brake.ReadValue<float>();
    public override float BrakeInput => IsFlipped ? PlayerControls.Vehicle.Gas.ReadValue<float>() : PlayerControls.Vehicle.Brake.ReadValue<float>();
    public override float HandbrakeInput => PlayerControls.Vehicle.Handbrake.ReadValue<float>();

    public PlayerInputHandler()
    {
        PlayerControls = new();
        PlayerControls.Enable();
    }
}
